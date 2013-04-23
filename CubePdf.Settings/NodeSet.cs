/* ------------------------------------------------------------------------- */
///
/// NodeSet.cs
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
    /// NodeSet
    /// 
    /// <summary>
    /// Node 用のコレクションクラスです。NodeSet クラスは、List(Node)
    /// コレクションに、名前で検索可能な各種メソッド (Contains, IndexOf,
    /// LastIndexOf, Find, FindLast) を追加したクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class NodeSet : List<Node>
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// NodeSet (constructor)
        /// 
        /// <summary>
        /// 空で、既定の初期量を備えた、NodeSet クラスの新しいインスタンスを
        /// 初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public NodeSet() : base() { }

        /* ----------------------------------------------------------------- */
        ///
        /// NodeSet (constructor)
        /// 
        /// <summary>
        /// 指定したコレクションからコピーした要素を格納し、コピーされる
        /// 要素の数を格納できるだけの容量を備えた、NodeSet クラスの新しい
        /// インスタンスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public NodeSet(IEnumerable<Node> cp) : base(cp) { }

        /* ----------------------------------------------------------------- */
        ///
        /// NodeSet (constructor)
        /// 
        /// <summary>
        /// 空で、指定した初期量を備えた、NodeSet クラスの新しいインスタンス
        /// を初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public NodeSet(Int32 capacity) : base(capacity) { }

        #endregion

        #region Public methods

        /* ----------------------------------------------------------------- */
        ///
        /// Contains
        /// 
        /// <summary>
        /// 指定した名前の設定されているオブジェクトがリスト内に存在するか
        /// どうかを判断します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool Contains(string name)
        {
            return base.Contains(new Node(name));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// IndexOf
        /// 
        /// <summary>
        /// 指定した名前の設定されているオブジェクトを検索し、リスト全体内で
        /// 最初に見つかった位置のインデックスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int IndexOf(string name)
        {
            return base.IndexOf(new Node(name));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// IndexOf
        /// 
        /// <summary>
        /// 指定した名前の設定されているオブジェクトを検索し、指定した
        /// インデックスから最後の要素までのリスト内の要素の範囲内で
        /// 最初に見つかった位置のインデックスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int IndexOf(string name, int index)
        {
            return base.IndexOf(new Node(name), index);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// IndexOf
        /// 
        /// <summary>
        /// 指定した名前の設定されているオブジェクトを検索し、指定した
        /// インデックスから始まって指定した数の要素を格納するリスト内の要素
        /// の範囲内で最初に見つかった位置のインデックスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int IndexOf(string name, int index, int count)
        {
            return base.IndexOf(new Node(name), index, count);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// LastIndexOf
        /// 
        /// <summary>
        /// 指定した名前の設定されているオブジェクトを検索し、リスト全体内で
        /// 最後に見つかった位置のインデックスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int LastIndexOf(string name)
        {
            return base.LastIndexOf(new Node(name));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// LastIndexOf
        /// 
        /// <summary>
        /// 指定した名前の設定されているオブジェクトを検索し、指定した
        /// インデックスから最後の要素までのリスト内の要素の範囲内で
        /// 最初に見つかった位置のインデックスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int LastIndexOf(string name, int index)
        {
            return base.LastIndexOf(new Node(name), index);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// LastIndexOf
        /// 
        /// <summary>
        /// 指定した名前の設定されているオブジェクトを検索し、指定した
        /// インデックスから始まって指定した数の要素を格納するリスト内の要素
        /// の範囲内で最初に見つかった位置のインデックスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int LastIndexOf(string name, int index, int count)
        {
            return base.LastIndexOf(new Node(name), index, count);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Find
        /// 
        /// <summary>
        /// 指定した名前の設定されているオブジェクトを検索し、リスト全体内で
        /// 最初に見つかった位置の要素を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Node Find(string name)
        {
            var dest = IndexOf(name);
            return (dest != -1) ? this[dest] : null;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Find
        /// 
        /// <summary>
        /// 指定した名前の設定されているオブジェクトを検索し、指定した
        /// インデックスから最後の要素までのリスト内の要素の範囲内で
        /// 最初に見つかった位置の要素を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Node Find(string name, int index)
        {
            var dest = IndexOf(name, index);
            return (dest != -1) ? this[dest] : null;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Find
        /// 
        /// <summary>
        /// 指定した名前の設定されているオブジェクトを検索し、指定した
        /// インデックスから始まって指定した数の要素を格納するリスト内の要素
        /// の範囲内で最初に見つかった位置の要素を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Node Find(string name, int index, int count)
        {
            var dest = IndexOf(name, index, count);
            return (dest != -1) ? this[dest] : null;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// FindLast
        /// 
        /// <summary>
        /// 指定した名前の設定されているオブジェクトを検索し、リスト全体内で
        /// 最後に見つかった位置の要素を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Node FindLast(string name)
        {
            var dest = LastIndexOf(name);
            return (dest != -1) ? this[dest] : null;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// FindLast
        /// 
        /// <summary>
        /// 指定した名前の設定されているオブジェクトを検索し、指定した
        /// インデックスから最後の要素までのリスト内の要素の範囲内で
        /// 最初に見つかった位置の要素を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Node FindLast(string name, int index)
        {
            var dest = LastIndexOf(name, index);
            return (dest != -1) ? this[dest] : null;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// FindLast
        /// 
        /// <summary>
        /// 指定した名前の設定されているオブジェクトを検索し、指定した
        /// インデックスから始まって指定した数の要素を格納するリスト内の要素
        /// の範囲内で最初に見つかった位置の要素を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Node FindLast(string name, int index, int count)
        {
            var dest = LastIndexOf(name, index, count);
            return (dest != -1) ? this[dest] : null;
        }

        #endregion
    }
}
