using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using System.Text;
using UnityEngine.Windows;

#if UNITY_EDITOR
[CustomEditor(typeof(CryptoComponent))]
public class CryptoEditor : Editor
{
    CryptoComponent _cryptoComponent;
    int _aesKeySize;
    string _aesKey;

    int _aesIVSize;
    string _aesIV;

    private void OnEnable()
    {
        _cryptoComponent = (CryptoComponent)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (string.IsNullOrEmpty(_aesKey))
        {
            if (string.IsNullOrEmpty(_cryptoComponent.aesBase64Key) == false)
            {
                _aesKey = Crypto.DecodingBase64(_cryptoComponent.aesBase64Key);
            }
            else
            {
                _aesKey = string.Empty;
            }
        }

        _aesKeySize = Encoding.UTF8.GetByteCount(_aesKey) * 8;
        EditorGUILayout.IntField("AES Key Size", _aesKeySize);
        _aesKey = EditorGUILayout.TextField("AES Key", _aesKey);
        EditorGUILayout.TextField("AES Base64 Key", _cryptoComponent.aesBase64Key);

        if (GUILayout.Button("Convert"))
        {
            bool isValid = false;
            string validKeySizeText = string.Empty;

            int validKeySize = 0;
            for (int i = 0, icount = CryptoAES.aesKeySize.Length; i < icount; i++)
            {
                validKeySize = CryptoAES.aesKeySize[i];
                validKeySizeText += validKeySize.ToString() + " ";
                if (_aesKeySize.Equals(validKeySize))
                {
                    isValid = true;
                    break;
                }
            }

            if (isValid)
            {
                _cryptoComponent.aesBase64Key = Crypto.EncodingBase64(_aesKey);
                EditorUtility.SetDirty(_cryptoComponent);
            }
            else
            {

                EditorUtility.DisplayDialog("key size error", string.Format("key size {0}", validKeySizeText), "ok");
            }
        }

        if (string.IsNullOrEmpty(_aesIV))
        {
            if (string.IsNullOrEmpty(_cryptoComponent.aesBase64IV) == false)
            {
                _aesIV = Crypto.DecodingBase64(_cryptoComponent.aesBase64IV);
            }
            else
            {
                _aesIV = string.Empty;
            }
        }

        _aesIVSize = Encoding.UTF8.GetByteCount(_aesIV) * 8;
        EditorGUILayout.IntField("AES IV Size", _aesIVSize);

        _aesIV = EditorGUILayout.TextField("AES IV", _aesIV);
        EditorGUILayout.TextField("AES Base64 Key", _cryptoComponent.aesBase64IV);

        if (GUILayout.Button("Convert"))
        {
            if (_aesIVSize == CryptoAES.aesIVSize)
            {
                _cryptoComponent.aesBase64IV = Crypto.EncodingBase64(_aesIV);
                EditorUtility.SetDirty(_cryptoComponent);
            }
            else
            {
                EditorUtility.DisplayDialog("iv size error", string.Format("IV Size {0}", CryptoAES.aesIVSize), "ok");
            }
        }
    }
}
#endif