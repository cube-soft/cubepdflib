/* ------------------------------------------------------------------------- */
///
/// EncryptionStatus.cs
///
/// Copyright (c) 2013 CubeSoft, Inc. All rights reserved.
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as published
/// by the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.
///
/* ------------------------------------------------------------------------- */
using System;

namespace CubePdf.Data
{
    /* --------------------------------------------------------------------- */
    ///
    /// EncryptionStatus
    /// 
    /// <summary>
    /// 暗号化されている PDF ファイルへのアクセス（許可）状態を定義した
    /// 列挙型です。各値の意味は以下の通りです。
    /// 
    /// NotEncrypted     : このファイルは暗号化されていません
    /// RestrictedAccess : ユーザパスワードで開いています
    /// FullAccess       : オーナパスワードで開いています
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public enum EncryptionStatus
    {
        NotEncrypted,
        RestrictedAccess,
        FullAccess,
    }
}
