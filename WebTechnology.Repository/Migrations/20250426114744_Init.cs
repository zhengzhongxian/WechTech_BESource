using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTechnology.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "actions",
                columns: table => new
                {
                    actionid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    action_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    metadata = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.actionid);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "brands",
                columns: table => new
                {
                    brand = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    brand_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    logo_data = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    website = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    manufacture_address = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    country = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.brand);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "order_status",
                columns: table => new
                {
                    status_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.status_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "parents",
                columns: table => new
                {
                    parentid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    parent_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    priority = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.parentid);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    paymentid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    payment_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.paymentid);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "product_status",
                columns: table => new
                {
                    status_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.status_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    roleid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    role_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    priority = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    metadata = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.roleid);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "sysdiagrams",
                columns: table => new
                {
                    diagram_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    principal_id = table.Column<int>(type: "int", nullable: false),
                    version = table.Column<int>(type: "int", nullable: true),
                    definition = table.Column<byte[]>(type: "longblob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.diagram_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "trends",
                columns: table => new
                {
                    trend = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trend_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    banner_data = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    priority = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    metadata = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.trend);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "units",
                columns: table => new
                {
                    unit = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    unit_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    unit_symbol = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    metadata = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.unit);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "user_status",
                columns: table => new
                {
                    status_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.status_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "vouchers",
                columns: table => new
                {
                    voucherid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    discount_value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    discount_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_date = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    end_date = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    usage_limit = table.Column<int>(type: "int", nullable: true),
                    used_count = table.Column<int>(type: "int", nullable: true),
                    min_order = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    max_discount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    metadata = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.voucherid);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    categoryid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    category_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    priority = table.Column<int>(type: "int", nullable: true),
                    parentid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    metadata = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.categoryid);
                    table.ForeignKey(
                        name: "FK_categories_parents",
                        column: x => x.parentid,
                        principalTable: "parents",
                        principalColumn: "parentid");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    orderid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order_number = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    customer_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order_date = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    shipping_address = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    shipping_fee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    shipping_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    total_price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    payment_method = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    status_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_success = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.orderid);
                    table.ForeignKey(
                        name: "FK_orders_order_status",
                        column: x => x.status_id,
                        principalTable: "order_status",
                        principalColumn: "status_id");
                    table.ForeignKey(
                        name: "FK_orders_payments",
                        column: x => x.payment_method,
                        principalTable: "payments",
                        principalColumn: "paymentid");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    permissionid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    roleid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    actionid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    metadata = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.permissionid);
                    table.ForeignKey(
                        name: "FK_permissions_actions",
                        column: x => x.actionid,
                        principalTable: "actions",
                        principalColumn: "actionid");
                    table.ForeignKey(
                        name: "FK_permissions_roles",
                        column: x => x.roleid,
                        principalTable: "roles",
                        principalColumn: "roleid");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    productid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    product_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    stockquantity = table.Column<int>(type: "int", nullable: true),
                    bar = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sku = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    brand = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    unit = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    status_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    metadata = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.productid);
                    table.ForeignKey(
                        name: "FK_products_brands",
                        column: x => x.brand,
                        principalTable: "brands",
                        principalColumn: "brand");
                    table.ForeignKey(
                        name: "FK_products_product_status",
                        column: x => x.status_id,
                        principalTable: "product_status",
                        principalColumn: "status_id");
                    table.ForeignKey(
                        name: "FK_products_units",
                        column: x => x.unit,
                        principalTable: "units",
                        principalColumn: "unit");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    userid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    username = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    otp = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    authenticate = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    verified_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    roleid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password_reset_token = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    refresh_token = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    status_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    count_Auth = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.userid);
                    table.ForeignKey(
                        name: "FK_users_roles",
                        column: x => x.roleid,
                        principalTable: "roles",
                        principalColumn: "roleid");
                    table.ForeignKey(
                        name: "FK_users_user_status",
                        column: x => x.status_id,
                        principalTable: "user_status",
                        principalColumn: "status_id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "apply_vouchers",
                columns: table => new
                {
                    applyid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    orderid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    voucherid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.applyid);
                    table.ForeignKey(
                        name: "FK_apply_vouchers_orders",
                        column: x => x.orderid,
                        principalTable: "orders",
                        principalColumn: "orderid");
                    table.ForeignKey(
                        name: "FK_apply_vouchers_vouchers",
                        column: x => x.voucherid,
                        principalTable: "vouchers",
                        principalColumn: "voucherid");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "order_log",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    old_status_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    new_status_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_log_orders",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "orderid");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "dimensions",
                columns: table => new
                {
                    dimensionid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    productid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    weight_value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    height_value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    width_value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    length_value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    metadata = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.dimensionid);
                    table.ForeignKey(
                        name: "FK_dimensions_products",
                        column: x => x.productid,
                        principalTable: "products",
                        principalColumn: "productid");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    imageid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image_data = table.Column<string>(type: "varchar(0)", maxLength: 0, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    productid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    metadata = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.imageid);
                    table.ForeignKey(
                        name: "FK_images_products",
                        column: x => x.productid,
                        principalTable: "products",
                        principalColumn: "productid");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "order_details",
                columns: table => new
                {
                    order_detail_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    product_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quantity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.order_detail_id);
                    table.ForeignKey(
                        name: "FK_order_details_orders",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "orderid");
                    table.ForeignKey(
                        name: "FK_order_details_products",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "productid");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "product_categories",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    productid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    categoryid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "FK_product_categories_categories",
                        column: x => x.categoryid,
                        principalTable: "categories",
                        principalColumn: "categoryid");
                    table.ForeignKey(
                        name: "FK_product_categories_products",
                        column: x => x.productid,
                        principalTable: "products",
                        principalColumn: "productid");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "product_prices",
                columns: table => new
                {
                    ppsid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    productid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    is_default = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ppsid);
                    table.ForeignKey(
                        name: "FK_product_prices_products",
                        column: x => x.productid,
                        principalTable: "products",
                        principalColumn: "productid");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "product_trends",
                columns: table => new
                {
                    ptsid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    productid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trend = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_date = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    end_date = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    metadata = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ptsid);
                    table.ForeignKey(
                        name: "FK_product_trends_products",
                        column: x => x.productid,
                        principalTable: "products",
                        principalColumn: "productid");
                    table.ForeignKey(
                        name: "FK_product_trends_trends",
                        column: x => x.trend,
                        principalTable: "trends",
                        principalColumn: "trend",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    customerid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    surname = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    middlename = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    firstname = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone_number = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    address = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    avatar = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dob = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    gender = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    metadata = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.customerid);
                    table.ForeignKey(
                        name: "FK_customers_users",
                        column: x => x.customerid,
                        principalTable: "users",
                        principalColumn: "userid");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "logs",
                columns: table => new
                {
                    logid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    userid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    actionid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description_details = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ip_address = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    useragent = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.logid);
                    table.ForeignKey(
                        name: "FK_logs_users",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "userid");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "carts",
                columns: table => new
                {
                    cartid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.cartid);
                    table.ForeignKey(
                        name: "FK_carts_customers",
                        column: x => x.cartid,
                        principalTable: "customers",
                        principalColumn: "customerid");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "review",
                columns: table => new
                {
                    reviewid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    customerid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    productid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rate = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.reviewid);
                    table.ForeignKey(
                        name: "FK_review_customers",
                        column: x => x.customerid,
                        principalTable: "customers",
                        principalColumn: "customerid");
                    table.ForeignKey(
                        name: "FK_review_products",
                        column: x => x.productid,
                        principalTable: "products",
                        principalColumn: "productid");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "cart_items",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cart_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    productid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quantity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "FK_cart_items_carts",
                        column: x => x.cart_id,
                        principalTable: "carts",
                        principalColumn: "cartid");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "comment",
                columns: table => new
                {
                    commentid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    comment_text = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reviewid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    commented_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    modified_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.commentid);
                    table.ForeignKey(
                        name: "FK_comment_review",
                        column: x => x.reviewid,
                        principalTable: "review",
                        principalColumn: "reviewid");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "FK_apply_vouchers_vouchers",
                table: "apply_vouchers",
                column: "voucherid");

            migrationBuilder.CreateIndex(
                name: "unique_order_voucher",
                table: "apply_vouchers",
                columns: new[] { "orderid", "voucherid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "unique_cart_product",
                table: "cart_items",
                columns: new[] { "cart_id", "productid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_categories_parents",
                table: "categories",
                column: "parentid");

            migrationBuilder.CreateIndex(
                name: "FK_comment_review",
                table: "comment",
                column: "reviewid");

            migrationBuilder.CreateIndex(
                name: "FK_dimensions_products",
                table: "dimensions",
                column: "productid");

            migrationBuilder.CreateIndex(
                name: "FK_images_products",
                table: "images",
                column: "productid");

            migrationBuilder.CreateIndex(
                name: "FK_logs_users",
                table: "logs",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "FK_order_details_products",
                table: "order_details",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "unique_order_product",
                table: "order_details",
                columns: new[] { "order_id", "product_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_order_log_orders",
                table: "order_log",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "FK_orders_order_status",
                table: "orders",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "FK_orders_payments",
                table: "orders",
                column: "payment_method");

            migrationBuilder.CreateIndex(
                name: "FK_permissions_actions",
                table: "permissions",
                column: "actionid");

            migrationBuilder.CreateIndex(
                name: "unique_role_action",
                table: "permissions",
                columns: new[] { "roleid", "actionid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_product_categories_categories",
                table: "product_categories",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "unique_product_category",
                table: "product_categories",
                columns: new[] { "productid", "categoryid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_product_prices_products",
                table: "product_prices",
                column: "productid");

            migrationBuilder.CreateIndex(
                name: "FK_product_trends_trends",
                table: "product_trends",
                column: "trend");

            migrationBuilder.CreateIndex(
                name: "unique_product_trend",
                table: "product_trends",
                columns: new[] { "productid", "trend" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_products_brands",
                table: "products",
                column: "brand");

            migrationBuilder.CreateIndex(
                name: "FK_products_product_status",
                table: "products",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "FK_products_units",
                table: "products",
                column: "unit");

            migrationBuilder.CreateIndex(
                name: "FK_review_products",
                table: "review",
                column: "productid");

            migrationBuilder.CreateIndex(
                name: "unique_customer_product",
                table: "review",
                columns: new[] { "customerid", "productid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_principal_name",
                table: "sysdiagrams",
                columns: new[] { "principal_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_users_roles",
                table: "users",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "FK_users_user_status",
                table: "users",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "unique_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "apply_vouchers");

            migrationBuilder.DropTable(
                name: "cart_items");

            migrationBuilder.DropTable(
                name: "comment");

            migrationBuilder.DropTable(
                name: "dimensions");

            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropTable(
                name: "logs");

            migrationBuilder.DropTable(
                name: "order_details");

            migrationBuilder.DropTable(
                name: "order_log");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "product_categories");

            migrationBuilder.DropTable(
                name: "product_prices");

            migrationBuilder.DropTable(
                name: "product_trends");

            migrationBuilder.DropTable(
                name: "sysdiagrams");

            migrationBuilder.DropTable(
                name: "vouchers");

            migrationBuilder.DropTable(
                name: "carts");

            migrationBuilder.DropTable(
                name: "review");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "actions");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "trends");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "order_status");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "parents");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "brands");

            migrationBuilder.DropTable(
                name: "product_status");

            migrationBuilder.DropTable(
                name: "units");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "user_status");
        }
    }
}
