/* ------------------------------------------------------------------------- */
///
/// Node.cs
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

namespace CubePdf.Settings
{
    /* --------------------------------------------------------------------- */
    ///
    /// Node
    /// 
    /// <summary>
    /// 最小単位の設定を格納するためのコンテナクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class Node : IEquatable<Node>
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// Node (constructor)
        /// 
        /// <summary>
        /// 既定の値でオブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Node() { }

        /* ----------------------------------------------------------------- */
        ///
        /// Node (constructor)
        /// 
        /// <summary>
        /// 引数にしていされた名前を利用して、オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Node(string name) { _name = name; }

        /* ----------------------------------------------------------------- */
        ///
        /// Node (constructor)
        /// 
        /// <summary>
        /// 引数にしていされた名前、および文字列の値を利用して、オブジェクト
        /// を初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Node(string name, string value)
        {
            _name = name;
            SetValue(value);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Node (constructor)
        /// 
        /// <summary>
        /// 引数にしていされた名前、および整数値を利用して、オブジェクトを
        /// 初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Node(string name, int value)
        {
            _name = name;
            SetValue(value);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Node (constructor)
        /// 
        /// <summary>
        /// 引数にしていされた名前、および真偽値を利用して、オブジェクトを
        /// 初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Node(string name, bool value)
        {
            _name = name;
            SetValue(value);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Node (constructor)
        /// 
        /// <summary>
        /// 引数にしていされた名前、およびコレクションの値を利用して、
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Node(string name, IList<Node> value)
        {
            _name = name;
            SetValue(value);
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Name
        /// 
        /// <summary>
        /// 設定の名前を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ValueKind
        /// 
        /// <summary>
        /// 設定の値の種類を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ValueKind ValueKind
        {
            get { return _kind; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ValueKind
        /// 
        /// <summary>
        /// 設定の値を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public object Value
        {
            get { return _value; }
        }

        #endregion

        #region Public methods

        /* ----------------------------------------------------------------- */
        ///
        /// GetValue
        /// 
        /// <summary>
        /// 設定の値を取得します。値の種類は、引数に指定されたデフォルト値の
        /// 型から判断します。値が設定されていない (null) 場合、および
        /// 指定されたデフォルト値と保存されている値の型が異なる場合は
        /// 指定されたデフォルト値が返ります。デフォルト値なしで値を取得する
        /// 場合は、Value プロパティから取得後、適切にキャストして下さい。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public T GetValue<T>(T default_value)
        {
            try
            {
                if (_value == null) return default_value;
                return (T)_value;
            }
            catch (InvalidCastException /* err */) { return default_value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SetValue
        /// 
        /// <summary>
        /// 整数値の値を設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void SetValue(int value)
        {
            _kind  = Settings.ValueKind.Number;
            _value = value;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SetValue
        /// 
        /// <summary>
        /// 文字列の値を設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void SetValue(string value)
        {
            _kind  = Settings.ValueKind.String;
            _value = value;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SetValue
        /// 
        /// <summary>
        /// 真偽値の値を設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void SetValue(bool value)
        {
            _kind  = Settings.ValueKind.Bool;
            _value = value;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SetValue
        /// 
        /// <summary>
        /// Node コレクションの値を設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void SetValue<T>(T value) where T : IList<Node>
        {
            _kind  = Settings.ValueKind.NodeSet;
            _value = value;
        }

        #endregion

        #region Implementations for IEquatable<Node>

        /* ----------------------------------------------------------------- */
        /// Equals
        /* ----------------------------------------------------------------- */
        public bool Equals(Node other)
        {
            return Name == other.Name;
        }

        /* ----------------------------------------------------------------- */
        /// Equals
        /* ----------------------------------------------------------------- */
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null)) return false;
            if (object.ReferenceEquals(this, obj)) return true;

            var other = obj as Node;
            if (other == null) return false;

            return this.Equals(other);
        }

        /* ----------------------------------------------------------------- */
        /// GetHashCode
        /* ----------------------------------------------------------------- */
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region Variables
        string _name = string.Empty;
        ValueKind _kind = ValueKind.Unknown;
        object _value = null;
        #endregion
    }
}
