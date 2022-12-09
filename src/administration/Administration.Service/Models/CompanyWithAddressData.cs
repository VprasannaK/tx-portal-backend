/********************************************************************************
 * Copyright (c) 2021,2022 BMW Group AG
 * Copyright (c) 2021,2022 Contributors to the CatenaX (ng) GitHub Organisation.
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

using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Enums;

namespace Org.Eclipse.TractusX.Portal.Backend.Administration.Service.Models;

/// <summary>
/// 
/// </summary>
/// <param name="CompanyId"></param>
/// <param name="Name"></param>
/// <param name="City"></param>
/// <param name="StreetName"></param>
/// <param name="CountryAlpha2Code"></param>
/// <param name="Bpn"></param>
/// <param name="ShortName"></param>
/// <param name="Region"></param>
/// <param name="StreetAdditional"></param>
/// <param name="StreetNumber"></param>
/// <param name="ZipCode"></param>
/// <param name="CountryDe"></param>
/// <param name="TaxId"></param>
/// <param name="CompanyRoles"></param>
/// <param name="CompanyUser"></param>
/// <returns></returns>

public record CompanyWithAddressData(Guid CompanyId,
    string Name,
    string City,
    string StreetName,
    string CountryAlpha2Code,
    string? Bpn,
    string? ShortName,
    string? Region,
    string? StreetAdditional,
    string? StreetNumber,
    string? ZipCode,
    string? CountryDe,
    string? TaxId,
    IEnumerable<AgreementsRoleData>? CompanyRoles,
    IEnumerable<CompanyUser>? CompanyUser
    );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="CompanyRole"></param>
    /// <param name="Agreements"></param>
    /// <returns></returns>
    public record AgreementsRoleData(CompanyRoleId CompanyRole, IEnumerable<Agreements> Agreements);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="AgreementId"></param>
    /// <param name="ConsentStatusId"></param>
    /// <returns></returns>
    public record Agreements(Guid AgreementId, string ConsentStatusId);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="FirstName"></param>
    /// <param name="LastName"></param>
    /// <param name="Email"></param>
    /// <returns></returns>
    public record CompanyUser(Guid UserId, string? FirstName, string? LastName, string? Email);

