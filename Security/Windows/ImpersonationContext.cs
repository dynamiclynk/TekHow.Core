using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using TekHow.Core.EventHandlers;

#if NETFRAMEWORK
namespace TekHow.Core.Security.Windows
{
    public class ImpersonationContext : IDisposable
    {
        #region imports
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain,
        string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseHandle(IntPtr handle);
        #endregion

        #region fields
        private const int Logon32ProviderDefault = 0;
        private const int Logon32LogonInteractive = 2;

        private readonly string _mDomain;
        private readonly string _mPassword;
        private readonly string _mUsername;
        private readonly bool _mHonorDispose;
        private IntPtr _mToken;

        private WindowsImpersonationContext _mContext = null;
        #endregion

        public event ImpersonationEventHandler Impersonated = null;

        #region properties
        private bool IsInContext => _mContext != null;
        #endregion

        #region ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain">Windows Active Directory domain name or workgroup name</param>
        /// <param name="username">Windows user name</param>
        /// <param name="password">User password (not required)</param>
        /// <param name="actionToImpersonate">Optional param to run your code in an Action delegate. Note if reflection is used you will need to access the Action.Target.</param>
        public ImpersonationContext(string domain, string username, string password = "", Action actionToImpersonate = null, bool honorDispose = false)
        {
            _mHonorDispose = honorDispose;
            _mDomain = domain;
            _mUsername = username;
            _mPassword = password;

            if (string.IsNullOrEmpty(_mDomain) | string.IsNullOrEmpty(_mUsername)) { throw new NullReferenceException("You must specify a  workgroup or domain and valid windows username."); }

            Enter();

            if (actionToImpersonate == null) return;
            try
            {
                actionToImpersonate?.Invoke();
            }
            finally
            {
                Dispose();
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Leaves the impersonation context and releases the unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_mHonorDispose)
            {
                Leave();
                _mContext?.Dispose();
                Impersonated?.Invoke(false);
            }
        }
        #endregion

        static int refCnt = 0;
        #region private methods
        [PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
        private void Enter()
        {
            refCnt++;

            if (this.IsInContext) return;

            if (System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToUpper() == $"{_mDomain}\\{_mUsername}".ToUpper())
            {
                return;
            }

            _mToken = new IntPtr(0);
            try
            {
                _mToken = IntPtr.Zero;
                var logonSuccessfull = LogonUser(
                    _mUsername,
                    _mDomain,
                    _mPassword,
                    Logon32LogonInteractive,
                    Logon32ProviderDefault,
                    ref _mToken);
                if (logonSuccessfull == false)
                {
                    var error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(error);
                }
                var identity = new WindowsIdentity(_mToken);
                _mContext = identity.Impersonate();

                var level = identity.ImpersonationLevel;

                Impersonated?.Invoke(true);

            }
            catch
            {
                Impersonated?.Invoke(true);
            }
        }

        private string GetProcessId()
        {
            return $"{Environment.MachineName} => {System.Diagnostics.Process.GetCurrentProcess().ProcessName}";
        }

        [PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
        private void Leave()
        {
            if (this.IsInContext == false) return;
            _mContext.Undo();

            if (_mToken != IntPtr.Zero) CloseHandle(_mToken);
            _mContext = null;

            Impersonated?.Invoke(false);
        }
        #endregion

    }
}
#endif