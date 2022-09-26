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

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Org.CatenaX.Ng.Portal.Backend.PortalBackend.PortalEntities.AuditEntities;

#nullable disable

namespace Org.CatenaX.Ng.Portal.Backend.PortalBackend.Migrations.Migrations
{
    public partial class CPLP1254dbaudit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM portal.company_service_account_assigned_roles;");
            migrationBuilder.Sql("DELETE FROM portal.company_user_assigned_roles;");
            migrationBuilder.Sql("DELETE FROM portal.user_roles;");
            
            migrationBuilder.AddColumn<Guid>(
                name: "last_editor_id",
                schema: "portal",
                table: "company_users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                schema: "portal",
                table: "company_assigned_apps",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<Guid>(
                name: "last_editor_id",
                schema: "portal",
                table: "company_assigned_apps",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "audit_company_assigned_apps_cplp_1254_db_audit",
                schema: "portal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    audit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_last_changed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    audit_operation_id = table.Column<int>(type: "integer", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    app_id = table.Column<Guid>(type: "uuid", nullable: false),
                    app_subscription_status_id = table.Column<int>(type: "integer", nullable: false),
                    requester_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_editor_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_company_assigned_apps_cplp_1254_db_audit", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "audit_company_users_cplp_1254_db_audit",
                schema: "portal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    audit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    audit_operation_id = table.Column<int>(type: "integer", nullable: false),
                    date_last_changed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    date_created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    firstname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    lastlogin = table.Column<byte[]>(type: "bytea", nullable: true),
                    lastname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_user_status_id = table.Column<int>(type: "integer", nullable: false),
                    last_editor_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_company_users_cplp_1254_db_audit", x => x.id);
                    table.ForeignKey(
                        name: "fk_audit_company_users_cplp_1254_db_audit_company_user_statuse",
                        column: x => x.company_user_status_id,
                        principalSchema: "portal",
                        principalTable: "company_user_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "audit_operation",
                schema: "portal",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    label = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_operation", x => x.id);
                });

            migrationBuilder.InsertData(
                schema: "portal",
                table: "audit_operation",
                columns: new[] { "id", "label" },
                values: new object[,]
                {
                    { 1, "INSERT" },
                    { 2, "UPDATE" },
                    { 3, "DELETE" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_audit_company_users_cplp_1254_db_audit_company_user_status_",
                schema: "portal",
                table: "audit_company_users_cplp_1254_db_audit",
                column: "company_user_status_id");
            
            migrationBuilder.AddAuditTrigger<AuditCompanyUser>("cplp_1254_db_audit");
            // migrationBuilder.AddAuditTrigger<AuditCompanyAssignedApp>("cplp_1254_db_audit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropAuditTrigger<AuditCompanyUser>();
            // migrationBuilder.DropAuditTrigger<AuditCompanyAssignedApp>();

            migrationBuilder.DropTable(
                name: "audit_company_assigned_apps_cplp_1254_db_audit",
                schema: "portal");

            migrationBuilder.DropTable(
                name: "audit_company_users_cplp_1254_db_audit",
                schema: "portal");

            migrationBuilder.DropTable(
                name: "audit_operation",
                schema: "portal");

            migrationBuilder.DropColumn(
                name: "last_editor_id",
                schema: "portal",
                table: "company_users");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "portal",
                table: "company_assigned_apps");

            migrationBuilder.DropColumn(
                name: "last_editor_id",
                schema: "portal",
                table: "company_assigned_apps");
        }
    }
}
