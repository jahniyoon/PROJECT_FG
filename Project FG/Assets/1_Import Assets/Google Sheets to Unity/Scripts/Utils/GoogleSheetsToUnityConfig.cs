using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using JH;
using System.IO;
using System.Text;

namespace GoogleSheetsToUnity
{
    public class GoogleSheetsToUnityConfig : ScriptableObject
    {
        public string CLIENT_ID = "";
        public string CLIENT_SECRETPW = "";
        public string CLIENT_SECRET => SECRET();

        public string ACCESS_TOKEN = "";


        [HideInInspector]
        public string REFRESH_TOKEN;

        public string API_Key = "";

        public int PORT;

        public GoogleDataResponse gdr;

        public string SECRET()
        {
            TextAsset key = Resources.Load("SECRETKEY") as TextAsset;
            if (key == null)
            {
                Debug.LogError("키를 찾을 수 없습니다. 키를 확인해주세요.");
                return "";
            }

            StringReader stringReader = new StringReader(key.text);
            string[] text = stringReader.ReadLine().Split();

            return Crypto.SimpleDecryptAES(CLIENT_SECRETPW, text[0], text[1]);
        }

    }

    [System.Serializable]
    public class GoogleDataResponse
    {
        public string ACCESS_TOKEN { get { return Decrypt(access_token); }}
        public string access_token;
        public string REFRESH_TOKEN { get { return Decrypt(refresh_token); }}
        public string refresh_token;

        public string token_type = "";
        public int expires_in = 0; //just a place holder to work the the json and caculate the next refresh time
        public DateTime nextRefreshTime;

        public void EncryptToken()
        {
            // 비어있으면 암호화를 하지 않는다.
            if(String.IsNullOrEmpty(access_token) == false)
            access_token = Encrypt(access_token);

            if(String.IsNullOrEmpty(refresh_token) == false)
            refresh_token = Encrypt(refresh_token);
        }  

        private string Encrypt(string token)
        {
            //Debug.Log("암호화 : " + token);
            TextAsset key = Resources.Load("SECRETKEY") as TextAsset;
            if (key == null)
            {
                Debug.LogError("키를 찾을 수 없습니다. 키를 확인해주세요.");
                return "";
            }

            StringReader stringReader = new StringReader(key.text);
            string[] text = stringReader.ReadLine().Split();

            return Crypto.EncryptAESbyBase64Key(token, text[0], text[1]);
        }

        public string Decrypt(string token)
        {
            TextAsset key = Resources.Load("SECRETKEY") as TextAsset;
            if (key == null)
            {
                Debug.LogError("키를 찾을 수 없습니다. 키를 확인해주세요.");
                return "";
            }

            StringReader stringReader = new StringReader(key.text);
            string[] text = stringReader.ReadLine().Split();

            return Crypto.SimpleDecryptAES(token, text[0], text[1]);
        }
    }

     

}