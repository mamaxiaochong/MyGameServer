﻿using System.Collections.Generic;
using MyGameServer.Model;

namespace MyGameServer.Cache
{
    //用户列表缓存，用来存所有用户模型及其对应客户端
    public class AccountsCache
    {
        //所有帐号ID和模型的映射
        private List<AccountModel> accountModelList;

        //在线玩家客户端和帐号模型映射
        private Dictionary<MyClientPeer, AccountModel> clientModelDict;

        public AccountsCache()
        {
            accountModelList = new List<AccountModel>();
            clientModelDict = new Dictionary<MyClientPeer, AccountModel>();

            //把数据库的都加到字典了
            accountModelList = SqlCache.GetAccountList();
        }

        #region 注册相关

        //添加帐号
        public void Add(string accountName, string password)
        {
            accountModelList.Add(new AccountModel(accountName, password));

            //添加到数据库
            SqlCache.AddAccount(accountName, password);
        }

        //缓存中是否已经含有该账户
        public bool Contain(string accountName)
        {
            foreach (AccountModel accountModel in accountModelList)
                if (accountName == accountModel.AccountName)
                    return true;

            return false;
        }

        //检测帐号密码是否匹配
        public bool IsMatch(string accountName, string password)
        {
            foreach (AccountModel accountModel in accountModelList)
                if (accountModel.AccountName == accountName && accountModel.Password == password)
                    return true;

            return false;
        }

        #endregion

        #region 登录相关

        //用户是否在线
        public bool IsOnline(string accountName)
        {
            foreach (AccountModel accountModel in clientModelDict.Values)
                if (accountName == accountModel.AccountName)
                    return true;
            return false;
        }

        //玩家上线
        public void OnLine(MyClientPeer client, string accountName, string password)
        {
            foreach (AccountModel accountModel in accountModelList)
                if (accountModel.AccountName == accountName && accountModel.Password == password)
                {
                    clientModelDict.Add(client, accountModel);
                    return;
                }
        }

        //玩家下线
        public void OffLine(MyClientPeer client)
        {
            clientModelDict.Remove(client);
        }

        //获取在线客户端的玩家模型
        public AccountModel GetAccountModel(MyClientPeer client)
        {
            AccountModel model = null;
            clientModelDict.TryGetValue(client, out model);
            return model;
        }


        #endregion

    }
}
