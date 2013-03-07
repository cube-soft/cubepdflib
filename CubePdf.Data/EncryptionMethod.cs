/* ------------------------------------------------------------------------- */
///
/// EncryptionMethod.cs
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

namespace CubePdf.Data
{
    /* --------------------------------------------------------------------- */
    ///
    /// EncryptionMethod
    /// 
    /// <summary>
    /// PDF の暗号化の際に使用可能な暗号化方式を定義した列挙型です。
    /// 現在のところ、以下の暗号化方式を使用する事ができます（括弧内の値は、
    /// 最初にサポートされた PDF バージョンを表します）。
    /// -  40bit RC4 (PDF 1.1)
    /// - 128bit RC4 (PDF 1.4)
    /// - 128bit AES (PDF 1.5)
    /// - 256bit AES (PDF 1.7 ExtensionLevel 3)
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public enum EncryptionMethod
    {
        Standard40,     //  40bit RC4
        Standard128,    // 128bit RC4
        Aes128,         // 128bit AES
        Aes256,         // 256bit AES
    }
}
