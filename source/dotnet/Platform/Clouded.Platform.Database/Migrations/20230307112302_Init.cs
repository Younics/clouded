using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Clouded.Platform.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        name = table.Column<string>(
                            type: "character varying(200)",
                            maxLength: 200,
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "regions",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        code = table.Column<string>(type: "text", nullable: false),
                        city = table.Column<string>(type: "text", nullable: false),
                        state = table.Column<string>(type: "text", nullable: false),
                        continent = table.Column<string>(type: "text", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_regions", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        name = table.Column<string>(
                            type: "character varying(200)",
                            maxLength: 200,
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "users",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        firstname = table.Column<string>(
                            name: "first_name",
                            type: "character varying(200)",
                            maxLength: 200,
                            nullable: false
                        ),
                        lastname = table.Column<string>(
                            name: "last_name",
                            type: "character varying(200)",
                            maxLength: 200,
                            nullable: false
                        ),
                        email = table.Column<string>(
                            type: "character varying(256)",
                            maxLength: 256,
                            nullable: false
                        ),
                        password = table.Column<string>(type: "text", nullable: false),
                        created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        createdticks = table.Column<long>(
                            name: "created_ticks",
                            type: "bigint",
                            nullable: false
                        ),
                        updated = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        updatedticks = table.Column<long>(
                            name: "updated_ticks",
                            type: "bigint",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        name = table.Column<string>(
                            type: "character varying(200)",
                            maxLength: 200,
                            nullable: false
                        ),
                        code = table.Column<string>(
                            type: "character varying(30)",
                            maxLength: 30,
                            nullable: false
                        ),
                        description = table.Column<string>(type: "text", nullable: true),
                        regionid = table.Column<long>(
                            name: "region_id",
                            type: "bigint",
                            nullable: false
                        ),
                        created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        createdticks = table.Column<long>(
                            name: "created_ticks",
                            type: "bigint",
                            nullable: false
                        ),
                        updated = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        updatedticks = table.Column<long>(
                            name: "updated_ticks",
                            type: "bigint",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.id);
                    table.ForeignKey(
                        name: "FK_projects_regions_region_id",
                        column: x => x.regionid,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "user_integrations",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        harboruser = table.Column<string>(
                            name: "harbor_user",
                            type: "text",
                            nullable: false
                        ),
                        harborpassword = table.Column<string>(
                            name: "harbor_password",
                            type: "text",
                            nullable: false
                        ),
                        harborproject = table.Column<string>(
                            name: "harbor_project",
                            type: "text",
                            nullable: false
                        ),
                        githuboauthtoken = table.Column<string>(
                            name: "github_oauth_token",
                            type: "text",
                            nullable: true
                        ),
                        userid = table.Column<long>(
                            name: "user_id",
                            type: "bigint",
                            nullable: false
                        ),
                        created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        createdticks = table.Column<long>(
                            name: "created_ticks",
                            type: "bigint",
                            nullable: false
                        ),
                        updated = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        updatedticks = table.Column<long>(
                            name: "updated_ticks",
                            type: "bigint",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_integrations", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_integrations_users_user_id",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "datasources",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        name = table.Column<string>(
                            type: "character varying(200)",
                            maxLength: 200,
                            nullable: false
                        ),
                        projectid = table.Column<long>(
                            name: "project_id",
                            type: "bigint",
                            nullable: false
                        ),
                        created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        createdticks = table.Column<long>(
                            name: "created_ticks",
                            type: "bigint",
                            nullable: false
                        ),
                        updated = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        updatedticks = table.Column<long>(
                            name: "updated_ticks",
                            type: "bigint",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_datasources", x => x.id);
                    table.ForeignKey(
                        name: "FK_datasources_projects_project_id",
                        column: x => x.projectid,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "project_user_relation",
                columns: table =>
                    new
                    {
                        projectid = table.Column<long>(
                            name: "project_id",
                            type: "bigint",
                            nullable: false
                        ),
                        userid = table.Column<long>(
                            name: "user_id",
                            type: "bigint",
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_project_user_relation",
                        x => new { x.userid, x.projectid }
                    );
                    table.ForeignKey(
                        name: "FK_project_user_relation_projects_project_id",
                        column: x => x.projectid,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_project_user_relation_users_user_id",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "configurations_datasource",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        type = table.Column<string>(type: "text", nullable: false),
                        server = table.Column<string>(type: "text", nullable: false),
                        port = table.Column<int>(type: "integer", nullable: false),
                        username = table.Column<string>(type: "text", nullable: false),
                        password = table.Column<string>(type: "text", nullable: false),
                        database = table.Column<string>(type: "text", nullable: false),
                        datasourceid = table.Column<long>(
                            name: "datasource_id",
                            type: "bigint",
                            nullable: false
                        ),
                        created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        createdticks = table.Column<long>(
                            name: "created_ticks",
                            type: "bigint",
                            nullable: false
                        ),
                        updated = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        updatedticks = table.Column<long>(
                            name: "updated_ticks",
                            type: "bigint",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configurations_datasource", x => x.id);
                    table.ForeignKey(
                        name: "FK_configurations_datasource_datasources_datasource_id",
                        column: x => x.datasourceid,
                        principalTable: "datasources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "providers",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        name = table.Column<string>(
                            type: "character varying(200)",
                            maxLength: 200,
                            nullable: false
                        ),
                        code = table.Column<string>(
                            type: "character varying(60)",
                            maxLength: 60,
                            nullable: false
                        ),
                        type = table.Column<string>(type: "text", nullable: false),
                        status = table.Column<string>(type: "text", nullable: false),
                        domainrecordid = table.Column<long>(
                            name: "domain_record_id",
                            type: "bigint",
                            nullable: true
                        ),
                        projectid = table.Column<long>(
                            name: "project_id",
                            type: "bigint",
                            nullable: false
                        ),
                        datasourceid = table.Column<long>(
                            name: "datasource_id",
                            type: "bigint",
                            nullable: true
                        ),
                        created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        createdticks = table.Column<long>(
                            name: "created_ticks",
                            type: "bigint",
                            nullable: false
                        ),
                        updated = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        updatedticks = table.Column<long>(
                            name: "updated_ticks",
                            type: "bigint",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_providers", x => x.id);
                    table.ForeignKey(
                        name: "FK_providers_datasources_datasource_id",
                        column: x => x.datasourceid,
                        principalTable: "datasources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_providers_projects_project_id",
                        column: x => x.projectid,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "admin_provider_datasource_relation",
                columns: table =>
                    new
                    {
                        adminproviderid = table.Column<long>(
                            name: "admin_provider_id",
                            type: "bigint",
                            nullable: false
                        ),
                        datasourceid = table.Column<long>(
                            name: "datasource_id",
                            type: "bigint",
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_admin_provider_datasource_relation",
                        x => new { x.datasourceid, x.adminproviderid }
                    );
                    table.ForeignKey(
                        name: "FK_admin_provider_datasource_relation_datasources_datasource_id",
                        column: x => x.datasourceid,
                        principalTable: "datasources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_admin_provider_datasource_relation_providers_admin_provider~",
                        column: x => x.adminproviderid,
                        principalTable: "providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "configurations_admin",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        apikey = table.Column<string>(
                            name: "api_key",
                            type: "text",
                            nullable: false
                        ),
                        providerid = table.Column<long>(
                            name: "provider_id",
                            type: "bigint",
                            nullable: false
                        ),
                        created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        createdticks = table.Column<long>(
                            name: "created_ticks",
                            type: "bigint",
                            nullable: false
                        ),
                        updated = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        updatedticks = table.Column<long>(
                            name: "updated_ticks",
                            type: "bigint",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configurations_admin", x => x.id);
                    table.ForeignKey(
                        name: "FK_configurations_admin_providers_provider_id",
                        column: x => x.providerid,
                        principalTable: "providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "configurations_auth",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        apikey = table.Column<string>(
                            name: "api_key",
                            type: "text",
                            nullable: false
                        ),
                        providerid = table.Column<long>(
                            name: "provider_id",
                            type: "bigint",
                            nullable: false
                        ),
                        created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        createdticks = table.Column<long>(
                            name: "created_ticks",
                            type: "bigint",
                            nullable: false
                        ),
                        updated = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        updatedticks = table.Column<long>(
                            name: "updated_ticks",
                            type: "bigint",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configurations_auth", x => x.id);
                    table.ForeignKey(
                        name: "FK_configurations_auth_providers_provider_id",
                        column: x => x.providerid,
                        principalTable: "providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "configurations_function",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        apikey = table.Column<string>(
                            name: "api_key",
                            type: "text",
                            nullable: false
                        ),
                        repositoryid = table.Column<string>(
                            name: "repository_id",
                            type: "text",
                            nullable: false
                        ),
                        repositorytype = table.Column<string>(
                            name: "repository_type",
                            type: "text",
                            nullable: false
                        ),
                        providerid = table.Column<long>(
                            name: "provider_id",
                            type: "bigint",
                            nullable: false
                        ),
                        created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        createdticks = table.Column<long>(
                            name: "created_ticks",
                            type: "bigint",
                            nullable: false
                        ),
                        updated = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        updatedticks = table.Column<long>(
                            name: "updated_ticks",
                            type: "bigint",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configurations_function", x => x.id);
                    table.ForeignKey(
                        name: "FK_configurations_function_providers_provider_id",
                        column: x => x.providerid,
                        principalTable: "providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "functions",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        executablename = table.Column<string>(
                            name: "executable_name",
                            type: "text",
                            nullable: false
                        ),
                        name = table.Column<string>(type: "text", nullable: false),
                        type = table.Column<string>(type: "text", nullable: false),
                        providerid = table.Column<long>(
                            name: "provider_id",
                            type: "bigint",
                            nullable: false
                        ),
                        created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        createdticks = table.Column<long>(
                            name: "created_ticks",
                            type: "bigint",
                            nullable: false
                        ),
                        updated = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        updatedticks = table.Column<long>(
                            name: "updated_ticks",
                            type: "bigint",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_functions", x => x.id);
                    table.ForeignKey(
                        name: "FK_functions_providers_provider_id",
                        column: x => x.providerid,
                        principalTable: "providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "configurations_auth_hash",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        algorithmtype = table.Column<string>(
                            name: "algorithm_type",
                            type: "text",
                            nullable: false
                        ),
                        secret = table.Column<string>(type: "text", nullable: false),
                        configurationid = table.Column<long>(
                            name: "configuration_id",
                            type: "bigint",
                            nullable: false
                        ),
                        argon2type = table.Column<string>(
                            name: "argon2_type",
                            type: "text",
                            nullable: true
                        ),
                        argon2version = table.Column<string>(
                            name: "argon2_version",
                            type: "text",
                            nullable: true
                        ),
                        argon2degreeofparallelism = table.Column<int>(
                            name: "argon2_degree_of_parallelism",
                            type: "integer",
                            nullable: true
                        ),
                        argon2memorysize = table.Column<int>(
                            name: "argon2_memory_size",
                            type: "integer",
                            nullable: true
                        ),
                        argon2iterations = table.Column<int>(
                            name: "argon2_iterations",
                            type: "integer",
                            nullable: true
                        ),
                        argon2returnbytes = table.Column<int>(
                            name: "argon2_return_bytes",
                            type: "integer",
                            nullable: true
                        ),
                        created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        createdticks = table.Column<long>(
                            name: "created_ticks",
                            type: "bigint",
                            nullable: false
                        ),
                        updated = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        updatedticks = table.Column<long>(
                            name: "updated_ticks",
                            type: "bigint",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configurations_auth_hash", x => x.id);
                    table.ForeignKey(
                        name: "FK_configurations_auth_hash_configurations_auth_configuration_~",
                        column: x => x.configurationid,
                        principalTable: "configurations_auth",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "configurations_auth_identity",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        identitytype = table.Column<string>(
                            name: "identity_type",
                            type: "text",
                            nullable: false
                        ),
                        schema = table.Column<string>(type: "text", nullable: false),
                        table = table.Column<string>(type: "text", nullable: false),
                        columnid = table.Column<string>(
                            name: "column_id",
                            type: "text",
                            nullable: false
                        ),
                        columnidentity = table.Column<string>(
                            name: "column_identity",
                            type: "text",
                            nullable: false
                        ),
                        organizationconfigurationid = table.Column<long>(
                            name: "organization_configuration_id",
                            type: "bigint",
                            nullable: true
                        ),
                        permissionconfigurationid = table.Column<long>(
                            name: "permission_configuration_id",
                            type: "bigint",
                            nullable: true
                        ),
                        roleconfigurationid = table.Column<long>(
                            name: "role_configuration_id",
                            type: "bigint",
                            nullable: true
                        ),
                        columnpassword = table.Column<string>(
                            name: "column_password",
                            type: "text",
                            nullable: true
                        ),
                        userconfigurationid = table.Column<long>(
                            name: "user_configuration_id",
                            type: "bigint",
                            nullable: true
                        ),
                        created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        createdticks = table.Column<long>(
                            name: "created_ticks",
                            type: "bigint",
                            nullable: false
                        ),
                        updated = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        updatedticks = table.Column<long>(
                            name: "updated_ticks",
                            type: "bigint",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configurations_auth_identity", x => x.id);
                    table.ForeignKey(
                        name: "FK_configurations_auth_identity_configurations_auth_organizati~",
                        column: x => x.organizationconfigurationid,
                        principalTable: "configurations_auth",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_configurations_auth_identity_configurations_auth_permission~",
                        column: x => x.permissionconfigurationid,
                        principalTable: "configurations_auth",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_configurations_auth_identity_configurations_auth_role_confi~",
                        column: x => x.roleconfigurationid,
                        principalTable: "configurations_auth",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_configurations_auth_identity_configurations_auth_user_confi~",
                        column: x => x.userconfigurationid,
                        principalTable: "configurations_auth",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "configurations_auth_token",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        validissuer = table.Column<string>(
                            name: "valid_issuer",
                            type: "text",
                            nullable: false
                        ),
                        validateissuer = table.Column<bool>(
                            name: "validate_issuer",
                            type: "boolean",
                            nullable: false
                        ),
                        validaudience = table.Column<string>(
                            name: "valid_audience",
                            type: "text",
                            nullable: false
                        ),
                        validateaudience = table.Column<bool>(
                            name: "validate_audience",
                            type: "boolean",
                            nullable: false
                        ),
                        validateissuersigningkey = table.Column<bool>(
                            name: "validate_issuer_signing_key",
                            type: "boolean",
                            nullable: false
                        ),
                        secret = table.Column<string>(type: "text", nullable: false),
                        accesstokenexpiration = table.Column<int>(
                            name: "access_token_expiration",
                            type: "integer",
                            nullable: false
                        ),
                        refreshtokenexpiration = table.Column<int>(
                            name: "refresh_token_expiration",
                            type: "integer",
                            nullable: false
                        ),
                        configurationid = table.Column<long>(
                            name: "configuration_id",
                            type: "bigint",
                            nullable: false
                        ),
                        created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        createdticks = table.Column<long>(
                            name: "created_ticks",
                            type: "bigint",
                            nullable: false
                        ),
                        updated = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        updatedticks = table.Column<long>(
                            name: "updated_ticks",
                            type: "bigint",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configurations_auth_token", x => x.id);
                    table.ForeignKey(
                        name: "FK_configurations_auth_token_configurations_auth_configuration~",
                        column: x => x.configurationid,
                        principalTable: "configurations_auth",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "provider_auth_function_relation",
                columns: table =>
                    new
                    {
                        authproviderid = table.Column<long>(
                            name: "auth_provider_id",
                            type: "bigint",
                            nullable: false
                        ),
                        functionid = table.Column<long>(
                            name: "function_id",
                            type: "bigint",
                            nullable: false
                        ),
                        index = table.Column<int>(type: "integer", nullable: false),
                        hooktype = table.Column<int>(
                            name: "hook_type",
                            type: "integer",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_provider_auth_function_relation",
                        x => new { x.functionid, x.authproviderid }
                    );
                    table.ForeignKey(
                        name: "FK_provider_auth_function_relation_functions_function_id",
                        column: x => x.functionid,
                        principalTable: "functions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_provider_auth_function_relation_providers_auth_provider_id",
                        column: x => x.authproviderid,
                        principalTable: "providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.InsertData(
                table: "regions",
                columns: new[] { "id", "city", "code", "continent", "state" },
                values: new object[] { 1L, "Bratislava", "EuSkBa", "Europe", "Slovakia" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_admin_provider_datasource_relation_admin_provider_id",
                table: "admin_provider_datasource_relation",
                column: "admin_provider_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_admin_provider_id",
                table: "configurations_admin",
                column: "provider_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_auth_provider_id",
                table: "configurations_auth",
                column: "provider_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_auth_hash_configuration_id",
                table: "configurations_auth_hash",
                column: "configuration_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_auth_identity_organization_configuration_id",
                table: "configurations_auth_identity",
                column: "organization_configuration_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_auth_identity_permission_configuration_id",
                table: "configurations_auth_identity",
                column: "permission_configuration_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_auth_identity_role_configuration_id",
                table: "configurations_auth_identity",
                column: "role_configuration_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_auth_identity_user_configuration_id",
                table: "configurations_auth_identity",
                column: "user_configuration_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_auth_token_configuration_id",
                table: "configurations_auth_token",
                column: "configuration_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_datasource_datasource_id",
                table: "configurations_datasource",
                column: "datasource_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_function_provider_id",
                table: "configurations_function",
                column: "provider_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_datasources_project_id",
                table: "datasources",
                column: "project_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_functions_provider_id",
                table: "functions",
                column: "provider_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_project_user_relation_project_id",
                table: "project_user_relation",
                column: "project_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_projects_code",
                table: "projects",
                column: "code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_projects_region_id",
                table: "projects",
                column: "region_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_provider_auth_function_relation_auth_provider_id",
                table: "provider_auth_function_relation",
                column: "auth_provider_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_providers_code",
                table: "providers",
                column: "code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_providers_datasource_id",
                table: "providers",
                column: "datasource_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_providers_project_id",
                table: "providers",
                column: "project_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_regions_code",
                table: "regions",
                column: "code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_user_integrations_user_id",
                table: "user_integrations",
                column: "user_id",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "admin_provider_datasource_relation");

            migrationBuilder.DropTable(name: "configurations_admin");

            migrationBuilder.DropTable(name: "configurations_auth_hash");

            migrationBuilder.DropTable(name: "configurations_auth_identity");

            migrationBuilder.DropTable(name: "configurations_auth_token");

            migrationBuilder.DropTable(name: "configurations_datasource");

            migrationBuilder.DropTable(name: "configurations_function");

            migrationBuilder.DropTable(name: "permissions");

            migrationBuilder.DropTable(name: "project_user_relation");

            migrationBuilder.DropTable(name: "provider_auth_function_relation");

            migrationBuilder.DropTable(name: "roles");

            migrationBuilder.DropTable(name: "user_integrations");

            migrationBuilder.DropTable(name: "configurations_auth");

            migrationBuilder.DropTable(name: "functions");

            migrationBuilder.DropTable(name: "users");

            migrationBuilder.DropTable(name: "providers");

            migrationBuilder.DropTable(name: "datasources");

            migrationBuilder.DropTable(name: "projects");

            migrationBuilder.DropTable(name: "regions");
        }
    }
}
