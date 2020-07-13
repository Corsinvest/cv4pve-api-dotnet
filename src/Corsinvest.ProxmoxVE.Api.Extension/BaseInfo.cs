/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 *
 * This source file is available under two different licenses:
 * - GNU General Public License version 3 (GPLv3)
 * - Corsinvest Enterprise License (CEL)
 * Full copyright and license information is available in
 * LICENSE.md which is distributed with this source code.
 *
 * Copyright (C) 2016 Corsinvest Srl	GPLv3 and CEL
 */

namespace Corsinvest.ProxmoxVE.Api.Extension
{
    /// <summary>
    /// Info base object.
    /// </summary>
    public class BaseInfo
    {
        internal PveClient Client { get; }

        /// <summary>
        /// Data Json API.
        /// </summary>
        /// <value></value>
        protected dynamic ApiData { get; }

        internal BaseInfo(PveClient client, object apiData) => (Client, ApiData) = (client, apiData);
    }
}