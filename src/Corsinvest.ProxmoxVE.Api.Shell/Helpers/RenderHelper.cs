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

using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Corsinvest.ProxmoxVE.Api.Metadata;

namespace Corsinvest.ProxmoxVE.Api.Shell.Helpers
{
    /// <summary>
    /// Render helper
    /// </summary>
    public class RenderHelper
    {
        /// <summary>
        /// Render value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <param name="returnParameters"></param>
        /// <returns></returns>
        public static object Value(object value, string key, List<ParameterApi> returnParameters)
        {
            if (returnParameters == null)
            {
                return (value is ExpandoObject || value is IList) ?
                        Newtonsoft.Json.JsonConvert.SerializeObject(value) :
                        value;
            }
            else
            {
                return returnParameters.Where(a => a.Name == key).FirstOrDefault().RendererValue(value);
            }
        }

    }
}