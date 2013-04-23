/* ------------------------------------------------------------------------- */
///
/// Document.cs
///
/// Copyright (c) 2013 CubeSoft, Inc. All rights reserved.
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU General Public License as published by
/// the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
///
/// You should have received a copy of the GNU General Public License
/// along with this program.  If not, see < http://www.gnu.org/licenses/ >.
///
/* ------------------------------------------------------------------------- */
using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace CubePdf.Settings
{
    /* --------------------------------------------------------------------- */
    ///
    /// Document
    /// 
    /// <summary>
    /// 設定の読み込み、および保存を行うためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class Document
    {
        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Root
        /// 
        /// <summary>
        /// 設定のルートとなる NodeSet オブジェクトを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public NodeSet Root
        {
            get { return _root; }
        }

        #endregion

        #region Public methods

        /* ----------------------------------------------------------------- */
        ///
        /// Clear
        /// 
        /// <summary>
        /// 格納されている全ての設定を消去します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Clear()
        {
            _root.Clear();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Read
        /// 
        /// <summary>
        /// レジストリから設定を読み込みます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Read(RegistryKey root)
        {
            _root.Clear();
            Read(root, _root);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Write
        /// 
        /// <summary>
        /// レジストリへ設定を保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Write(RegistryKey root)
        {
            Write(_root, root);
        }

        #endregion

        #region Other methods

        /* ----------------------------------------------------------------- */
        ///
        /// Read
        /// 
        /// <summary>
        /// レジストリから設定を読み込んで、引数に指定された NodeSet
        /// オブジェクトに格納します。。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void Read(RegistryKey src, NodeSet dest)
        {
            foreach (var name in src.GetSubKeyNames())
            {
                var subkey = src.OpenSubKey(name, false);
                var node = new Node(name);
                node.SetValue(new NodeSet());
                dest.Add(node);
                Read(subkey, node.Value as NodeSet);
            }
            ReadValues(src, dest);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ReadValues
        /// 
        /// <summary>
        /// レジストリから設定を読み込んで、引数に指定された NodeSet
        /// オブジェクトに格納します。。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void ReadValues(RegistryKey src, NodeSet dest)
        {
            foreach (var name in src.GetValueNames())
            {
                var node = new Node(name);
                switch (src.GetValueKind(name))
                {
                case RegistryValueKind.Binary:
                    var bytes = (byte[])src.GetValue(name);
                    node.SetValue(bytes.Length > 0 && bytes[0] != 0);
                    break;
                case RegistryValueKind.DWord:
                    node.SetValue((int)src.GetValue(name));
                    break;
                case RegistryValueKind.String:
                    node.SetValue((string)src.GetValue(name));
                    break;
                default:
                    break;
                }
                dest.Add(node);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Write
        /// 
        /// <summary>
        /// レジストリへ設定を保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void Write(IList<Node> src, RegistryKey dest)
        {
            foreach (var node in src)
            {
                switch (node.ValueKind)
                {
                case ValueKind.NodeSet:
                    Write(node.Value as IList<Node>, dest.CreateSubKey(node.Name));
                    break;
                case ValueKind.String:
                case ValueKind.Number:
                    dest.SetValue(node.Name, node.Value);
                    break;
                case ValueKind.Bool:
                    dest.SetValue(node.Name, (bool)node.Value ? _true : _false);
                    break;
                default:
                    break;
                }
            }
        }

        #endregion

        #region Variables
        private NodeSet _root = new NodeSet();
        private byte[] _true = { 1 };
        private byte[] _false = { 0 };
        #endregion
    }
}
