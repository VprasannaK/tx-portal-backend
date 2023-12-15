/********************************************************************************
 * Copyright (c) 2021, 2023 Contributors to the Eclipse Foundation
 *
 * See the NOTICE file(s) distributed with this work for additional
 * information regarding copyright ownership.
 *
 * This program and the accompanying materials are made available under the
 * terms of the Apache License, Version 2.0 which is available at
 * https://www.apache.org/licenses/LICENSE-2.0.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 * SPDX-License-Identifier: Apache-2.0
 ********************************************************************************/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Identities;

namespace Org.Eclipse.TractusX.Portal.Backend.Framework.ProcessIdentity.DependencyInjection;

public static class ProcessIdentityServiceCollectionExtensions
{
    public static IServiceCollection AddConfigurationProcessIdentityIdDetermination(this IServiceCollection services, IConfigurationSection section)
    {
        services.AddOptions<ProcessIdentitySettings>()
            .Bind(section)
            .ValidateOnStart();

        return services
            .AddTransient<IProcessIdentityDataBuilder, ProcessIdentityDataBuilder>()
            .AddTransient<IIdentityService, ProcessIdentityService>();
    }

    public static IServiceCollection AddConfigurationProcessIdentityService(this IServiceCollection services, IConfigurationSection section)
    {
        services.AddOptions<ProcessIdentitySettings>()
            .Bind(section)
            .ValidateOnStart();

        return services
            .AddScoped<IProcessIdentityDataBuilder, ProcessIdentityDataBuilder>()
            .AddTransient<IProcessIdentityDataDetermination, ProcessIdentityDataDetermination>()
            .AddTransient<IIdentityService, ProcessIdentityService>();
    }
}
