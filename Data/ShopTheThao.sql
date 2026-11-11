/* =====================================================================
   ĐỒ ÁN: WEBSITE QUẢN LÝ MẶT HÀNG THỂ THAO (ShopTheThao - ASP.NET MVC 5)
   File: SportShop.sql  |  SQL Server / LocalDB
   Cách chạy: mở bằng SSMS, kết nối (localdb)\MSSQLLocalDB rồi bấm Execute (F5).
   Script an toàn: tạo mới nếu chưa có, không xoá dữ liệu đã có.
   Tài khoản mẫu:  admin@sportshop.vn / Admin@123
                   khach@sportshop.vn / Khach@123
   ===================================================================== */

USE master;
GO
IF DB_ID(N'ShopTheThao') IS NULL
    CREATE DATABASE ShopTheThao;
GO
USE ShopTheThao;
GO

/* ===================== 1. TẠO BẢNG ===================== */

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
CREATE TABLE dbo.Users (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    FullName     NVARCHAR(100) NOT NULL,
    Email        NVARCHAR(150) NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Phone        NVARCHAR(15)  NULL,
    Address      NVARCHAR(250) NULL,
    AvatarUrl    NVARCHAR(MAX) NULL,
    Role         NVARCHAR(20)  NOT NULL DEFAULT N'Customer',   -- Admin | Customer
    IsActive     BIT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Users_Email UNIQUE (Email)
);
GO

IF OBJECT_ID(N'dbo.Categories', N'U') IS NULL
CREATE TABLE dbo.Categories (
    Id       INT IDENTITY(1,1) PRIMARY KEY,
    Name     NVARCHAR(100) NOT NULL,
    Slug     NVARCHAR(200) NULL,
    ImageUrl NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

IF OBJECT_ID(N'dbo.Brands', N'U') IS NULL
CREATE TABLE dbo.Brands (
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    Name    NVARCHAR(100) NOT NULL,
    LogoUrl NVARCHAR(MAX) NULL
);
GO

IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
CREATE TABLE dbo.Products (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Code        NVARCHAR(20)  NULL,                 -- Mã SP (SP001...) cũng là tên file ảnh
    Name        NVARCHAR(200) NOT NULL,
    Slug        NVARCHAR(200) NULL,
    Description NVARCHAR(MAX) NULL,
    Price       DECIMAL(18,0) NOT NULL,
    SalePrice   DECIMAL(18,0) NULL,
    ImageUrl    NVARCHAR(MAX) NULL,
    Images      NVARCHAR(MAX) NULL,                 -- nhiều ảnh, ngăn cách bằng ';'
    CategoryId  INT NOT NULL,
    BrandId     INT NULL,
    IsActive    BIT NOT NULL DEFAULT 1,
    IsFeatured  BIT NOT NULL DEFAULT 0,
    SoldCount   INT NOT NULL DEFAULT 0,
    CreatedAt   DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(Id),
    CONSTRAINT FK_Products_Brands     FOREIGN KEY (BrandId)    REFERENCES dbo.Brands(Id)
);
GO

IF OBJECT_ID(N'dbo.ProductVariants', N'U') IS NULL
CREATE TABLE dbo.ProductVariants (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    Size      NVARCHAR(20) NULL,
    Color     NVARCHAR(30) NULL,
    Stock     INT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Variants_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
);
GO

IF OBJECT_ID(N'dbo.Carts', N'U') IS NULL
CREATE TABLE dbo.Carts (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    UserId    INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Carts_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
);
GO

IF OBJECT_ID(N'dbo.CartItems', N'U') IS NULL
CREATE TABLE dbo.CartItems (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    CartId           INT NOT NULL,
    ProductVariantId INT NOT NULL,
    Quantity         INT NOT NULL DEFAULT 1,
    CONSTRAINT FK_CartItems_Carts    FOREIGN KEY (CartId)           REFERENCES dbo.Carts(Id),
    CONSTRAINT FK_CartItems_Variants FOREIGN KEY (ProductVariantId) REFERENCES dbo.ProductVariants(Id)
);
GO

IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
CREATE TABLE dbo.Orders (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    UserId         INT NOT NULL,
    OrderDate      DATETIME NOT NULL DEFAULT GETDATE(),
    Status         NVARCHAR(20) NOT NULL DEFAULT N'Pending',  -- Pending|Confirmed|Shipping|Completed|Cancelled
    ReceiverName   NVARCHAR(100) NULL,
    Phone          NVARCHAR(15)  NULL,
    Address        NVARCHAR(250) NULL,
    PaymentMethod  NVARCHAR(20)  NULL,                        -- COD | Transfer
    CouponCode     NVARCHAR(30)  NULL,
    DiscountAmount DECIMAL(18,0) NOT NULL DEFAULT 0,
    Total          DECIMAL(18,0) NOT NULL DEFAULT 0,
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
);
GO

IF OBJECT_ID(N'dbo.OrderItems', N'U') IS NULL
CREATE TABLE dbo.OrderItems (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    OrderId     INT NOT NULL,
    ProductId   INT NOT NULL,
    ProductName NVARCHAR(200) NULL,
    Size        NVARCHAR(20)  NULL,
    Color       NVARCHAR(30)  NULL,
    UnitPrice   DECIMAL(18,0) NOT NULL,
    Quantity    INT NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id)
);
GO

IF OBJECT_ID(N'dbo.Coupons', N'U') IS NULL
CREATE TABLE dbo.Coupons (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Code            NVARCHAR(30) NOT NULL,
    DiscountPercent INT NOT NULL,
    MaxDiscount     DECIMAL(18,0) NULL,
    MinOrder        DECIMAL(18,0) NOT NULL DEFAULT 0,
    ExpireDate      DATETIME NOT NULL,
    UsageLimit      INT NOT NULL DEFAULT 0,
    UsedCount       INT NOT NULL DEFAULT 0,
    IsActive        BIT NOT NULL DEFAULT 1,
    CONSTRAINT UQ_Coupons_Code UNIQUE (Code)
);
GO

IF OBJECT_ID(N'dbo.Wishlists', N'U') IS NULL
CREATE TABLE dbo.Wishlists (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    UserId    INT NOT NULL,
    ProductId INT NOT NULL,
    CONSTRAINT FK_Wishlists_Users    FOREIGN KEY (UserId)    REFERENCES dbo.Users(Id),
    CONSTRAINT FK_Wishlists_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id),
    CONSTRAINT UQ_Wishlists_UserProduct UNIQUE (UserId, ProductId)
);
GO

IF OBJECT_ID(N'dbo.Reviews', N'U') IS NULL
CREATE TABLE dbo.Reviews (
    Id         INT IDENTITY(1,1) PRIMARY KEY,
    ProductId  INT NOT NULL,
    UserId     INT NOT NULL,
    Rating     INT NOT NULL,
    Comment    NVARCHAR(MAX) NULL,
    CreatedAt  DATETIME NOT NULL DEFAULT GETDATE(),
    IsApproved BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Reviews_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id),
    CONSTRAINT FK_Reviews_Users    FOREIGN KEY (UserId)    REFERENCES dbo.Users(Id)
);
GO

IF OBJECT_ID(N'dbo.Contacts', N'U') IS NULL
CREATE TABLE dbo.Contacts (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    Name      NVARCHAR(100) NULL,
    Email     NVARCHAR(150) NULL,
    Message   NVARCHAR(MAX) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsRead    BIT NOT NULL DEFAULT 0
);
GO

IF OBJECT_ID(N'dbo.InventoryLogs', N'U') IS NULL
CREATE TABLE dbo.InventoryLogs (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    ProductVariantId INT NOT NULL,
    Change           INT NOT NULL,
    Type             NVARCHAR(20) NULL,       -- Import | Export
    Note             NVARCHAR(250) NULL,
    CreatedAt        DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_InvLogs_Variants FOREIGN KEY (ProductVariantId) REFERENCES dbo.ProductVariants(Id)
);
GO

IF OBJECT_ID(N'dbo.Banners', N'U') IS NULL
CREATE TABLE dbo.Banners (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    Title     NVARCHAR(200) NULL,
    ImageUrl  NVARCHAR(MAX) NULL,
    Link      NVARCHAR(MAX) NULL,
    SortOrder INT NOT NULL DEFAULT 0,
    IsActive  BIT NOT NULL DEFAULT 1
);
GO

/* ===================== 2. DỮ LIỆU MẪU ===================== */

-- Tài khoản (mật khẩu đã mã hoá theo ASP.NET Identity PasswordHasher)
IF NOT EXISTS (SELECT 1 FROM dbo.Users)
BEGIN
    INSERT INTO dbo.Users (FullName, Email, PasswordHash, Phone, Role, IsActive)
    VALUES (N'Quản trị viên', N'admin@sportshop.vn', N'AH5hwr8OO3qYrRQJdWr3s8dNgThYocVS0fuLicOijTEjNEtQHzo2Wqa7XBNR0Vx5XQ==', N'0900000001', N'Admin', 1),
           (N'Khách hàng mẫu', N'khach@sportshop.vn', N'AB8F9qqoxpUWTch4otAFaZC8mTYezSXbRDh1H06QnThT56kjlqNQPaPwoNk0KIKdHA==', N'0900000002', N'Customer', 1);
END
GO

-- Danh mục + thương hiệu
IF NOT EXISTS (SELECT 1 FROM dbo.Categories)
BEGIN
    INSERT INTO dbo.Categories (Name, Slug, IsActive) VALUES
        (N'Bóng đá', N'bong-da', 1), (N'Chạy bộ', N'chay-bo', 1), (N'Gym', N'gym', 1),
        (N'Cầu lông', N'cau-long', 1), (N'Phụ kiện', N'phu-kien', 1);
END
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Brands)
BEGIN
    INSERT INTO dbo.Brands (Name) VALUES (N'Nike'), (N'Adidas'), (N'Puma'), (N'Yonex');
END
GO

-- Sản phẩm + biến thể (size, màu, tồn kho)
IF NOT EXISTS (SELECT 1 FROM dbo.Products)
BEGIN
    DECLARE @bd INT = (SELECT Id FROM dbo.Categories WHERE Slug = N'bong-da');
    DECLARE @cb INT = (SELECT Id FROM dbo.Categories WHERE Slug = N'chay-bo');
    DECLARE @gy INT = (SELECT Id FROM dbo.Categories WHERE Slug = N'gym');
    DECLARE @cl INT = (SELECT Id FROM dbo.Categories WHERE Slug = N'cau-long');
    DECLARE @nike INT = (SELECT Id FROM dbo.Brands WHERE Name = N'Nike');
    DECLARE @adi  INT = (SELECT Id FROM dbo.Brands WHERE Name = N'Adidas');
    DECLARE @puma INT = (SELECT Id FROM dbo.Brands WHERE Name = N'Puma');
    DECLARE @yon  INT = (SELECT Id FROM dbo.Brands WHERE Name = N'Yonex');

    INSERT INTO dbo.Products (Code, Name, Slug, Description, Price, SalePrice, CategoryId, BrandId, IsFeatured, SoldCount) VALUES
     (N'SP001', N'Giày đá bóng Nike Mercurial', N'giay-nike-mercurial', N'Giày đá bóng sân cỏ nhân tạo, nhẹ và bám sân tốt.', 2500000, 2190000, @bd, @nike, 1, 40),
     (N'SP002', N'Giày chạy bộ Adidas Ultraboost', N'adidas-ultraboost', N'Đệm Boost êm ái cho mọi cự ly.', 3200000, NULL, @cb, @adi, 1, 65),
     (N'SP003', N'Vợt cầu lông Yonex Astrox', N'yonex-astrox', N'Vợt thiên công, khung carbon.', 1800000, 1590000, @cl, @yon, 0, 30),
     (N'SP004', N'Áo tập gym Puma Dry', N'ao-gym-puma', N'Vải thoáng khí, nhanh khô.', 450000, NULL, @gy, @puma, 0, 90);

    INSERT INTO dbo.ProductVariants (ProductId, Size, Color, Stock)
    SELECT p.Id, s.Size, N'Cam', 20
    FROM dbo.Products p
    CROSS JOIN (VALUES (N'39'), (N'40'), (N'41'), (N'42')) AS s(Size)
    WHERE p.Slug IN (N'giay-nike-mercurial', N'adidas-ultraboost', N'ao-gym-puma');

    INSERT INTO dbo.ProductVariants (ProductId, Size, Color, Stock)
    SELECT Id, N'4U', N'Đen', 15 FROM dbo.Products WHERE Slug = N'yonex-astrox';
END
GO

-- Mã giảm giá + banner
IF NOT EXISTS (SELECT 1 FROM dbo.Coupons)
    INSERT INTO dbo.Coupons (Code, DiscountPercent, MaxDiscount, MinOrder, ExpireDate, UsageLimit, UsedCount, IsActive)
    VALUES (N'SPORT10', 10, NULL, 500000, DATEADD(MONTH, 3, GETDATE()), 100, 0, 1);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Banners)
    INSERT INTO dbo.Banners (Title, SortOrder, IsActive) VALUES (N'Trang bị cho mọi buổi tập', 1, 1);
GO

/* ===================== 3. ẢNH SẢN PHẨM =====================
   Ảnh KHÔNG lưu trong SQL. Quy ước: tên file ảnh = Mã SP (cột Code).
   Chép ảnh vào thư mục Content/images/products/ của project:
       SP001.jpg  SP002.jpg  SP003.jpg  SP004.jpg   (hoặc .png / .webp)
   Ảnh phụ cho trang chi tiết: SP001-1.jpg, SP001-2.jpg, SP001-3.jpg ...
   Thêm sản phẩm mới: đặt Code mới (SP005...) rồi chép file SP005.jpg là xong.
   ============================================================ */

/* Kiểm tra nhanh */
SELECT N'Users' AS [Bảng], COUNT(*) AS [Số dòng] FROM dbo.Users
UNION ALL SELECT N'Categories', COUNT(*) FROM dbo.Categories
UNION ALL SELECT N'Brands', COUNT(*) FROM dbo.Brands
UNION ALL SELECT N'Products', COUNT(*) FROM dbo.Products
UNION ALL SELECT N'ProductVariants', COUNT(*) FROM dbo.ProductVariants
UNION ALL SELECT N'Coupons', COUNT(*) FROM dbo.Coupons
UNION ALL SELECT N'Banners', COUNT(*) FROM dbo.Banners;
GO