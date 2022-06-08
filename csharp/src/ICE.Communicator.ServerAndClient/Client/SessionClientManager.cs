using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmCommunicator.Client
{
    class SessionClientManager
    {
        private int _type = 0;
        //ice通信对象
        private IceClient _ice = null;
        //事件通知对象
        private EventClientHelper _eventHelper = null;

        public SessionClientManager(int type)
        {
            _type = type;
        }

        /// <summary>
        /// 初始化通信
        /// </summary>
        /// <param name="clientId">登录id</param>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        /// <param name="type">通信种类</param>
        /// <param name="mode">通信模式 one way or two way</param>
        /// <returns></returns>
        public int Init(string clientId, string ip, string port, char mode, ConnectionEventClientHandler handler)
        {
            _eventHelper = new EventClientHelper();
            //注册连接事件
            _eventHelper.RegConnectionEvent(handler);
            _ice = new IceClient(mode, _eventHelper);
            //启动ice连接
            return _ice.Init(clientId, ip, port);
        }
    }
}
