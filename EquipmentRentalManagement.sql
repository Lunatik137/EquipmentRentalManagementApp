USE [master]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'EquipmentRentalManagement')
BEGIN
	ALTER DATABASE EquipmentRentalManagement SET OFFLINE WITH ROLLBACK IMMEDIATE;
	ALTER DATABASE EquipmentRentalManagement SET ONLINE;
	DROP DATABASE EquipmentRentalManagement;
END
GO

CREATE DATABASE EquipmentRentalManagement
GO

USE EquipmentRentalManagement
GO

-- Tạo bảng Equipment
CREATE TABLE Equipment (
    EquipmentID INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    Description NVARCHAR(255),
    Category NVARCHAR(50),
    Status BIT,
    QuantityAvailable INT,
    DailyRate DECIMAL(10,2),
    PurchaseDate DATE
);

-- Tạo bảng User
CREATE TABLE [User] (
    UserId INT PRIMARY KEY IDENTITY,
    FullName NVARCHAR(100),
    Username NVARCHAR(25),
    Password NVARCHAR(25),
    Phone NVARCHAR(10),
    Email NVARCHAR(100),
    IsActive BIT NOT NULL DEFAULT 1,
    Role NVARCHAR(20) NOT NULL DEFAULT 'Staff'
);

-- Tạo bảng RentalContract
CREATE TABLE RentalContract (
    ContractID INT PRIMARY KEY IDENTITY,
    UserId INT,
    StartDate DATE,
    EndDate DATE,
    ReturnDate DATE,
    TotalAmount DECIMAL(10,2),
    Status NVARCHAR(20),
    FOREIGN KEY (UserId) REFERENCES [User](UserId)
);

-- Tạo bảng RentalDetail
CREATE TABLE RentalDetail (
    RentalDetailID INT PRIMARY KEY IDENTITY,
    ContractID INT,
    EquipmentID INT,
    Quantity INT,
    RatePerDay DECIMAL(10,2),
    FOREIGN KEY (ContractID) REFERENCES RentalContract(ContractID),
    FOREIGN KEY (EquipmentID) REFERENCES Equipment(EquipmentID)
);

-- Tạo bảng Payment
CREATE TABLE Payment (
    PaymentID INT PRIMARY KEY IDENTITY,
    ContractID INT,
    PaymentDate DATE,
    AmountPaid DECIMAL(10,2),
    Note NVARCHAR(255),
    FOREIGN KEY (ContractID) REFERENCES RentalContract(ContractID)
);

-- Dữ liệu mẫu
INSERT INTO Equipment (Name, Description, Category, Status, DailyRate, PurchaseDate, QuantityAvailable) VALUES
(N'Máy khoan Bosch GSB 13RE', N'Máy khoan cầm tay đa năng', N'Công cụ điện', N'1', 50000, '2023-03-01', 10),
(N'Máy cắt sắt Makita', N'Dùng cắt kim loại công nghiệp', N'Công cụ điện', N'1', 70000, '2022-12-15', 5),
(N'Máy phát điện Honda 2KW', N'Máy phát điện chạy xăng', N'Năng lượng', N'1', 150000, '2023-01-10', 3),
(N'Bộ giàn giáo 2m', N'Dành cho thi công ngoài trời', N'Xây dựng', N'0', 80000, '2022-10-20', 8),
(N'Xe rùa', N'Xe rùa vận chuyển vật liệu', N'Xây dựng', N'1', 30000, '2023-04-05', 15),
(N'Máy cắt gạch', N'Máy cắt gạch bằng tay', N'Công cụ xây dựng', N'0', 40000, '2023-02-18', 6),
(N'Máy hàn điện tử Jasic', N'Máy hàn mini dùng điện', N'Công cụ điện', N'1', 60000, '2023-01-25', 4),
(N'Máy hút bụi công nghiệp', N'Dùng trong công trình xây dựng', N'Ve sinh', N'0', 90000, '2022-11-12', 2),
(N'Đèn chiếu sáng công trình', N'Đèn LED công suất lớn', N'Chiếu sáng', N'1', 20000, '2023-03-10', 20),
(N'Búa phá bê tông', N'Dùng để đục phá sàn', N'Xây dựng', N'1', 100000, '2022-08-30', 3),
(N'Máy cân mực laser', N'Cân chỉnh mặt phẳng thi công', N'Dụng cụ đo', N'1', 50000, '2023-04-01', 5),
(N'Máy cắt cỏ Honda', N'Dành cho làm vườn', N'Làm vườn', N'1', 70000, '2023-05-12', 7),
(N'Bình xịt sơn', N'Xịt sơn tường, gỗ...', N'Dụng cụ hoàn thiện', N'1', 40000, '2023-03-20', 10),
(N'Máy rửa xe cao áp', N'Rửa xe, thiết bị công trình', N'Ve sinh', N'1', 80000, '2023-02-05', 3),
(N'Máy đo khoảng cách laser', N'Do khoảng cách chính xác cao', N'Dụng cụ đo', N'1', 60000, '2023-01-30', 4);

-- Thêm người dùng với vai trò Owner
INSERT INTO [User] (FullName, Username, Password, Phone, Email, IsActive, Role)
VALUES 
(N'Chủ 1', 'owner1', 'ownerpass1', '0900000001', 'owner1@email.com', 1, 'Owner'),
(N'Nhân viên 1', 'staff1', 'staffpass2', '0900000002', 'staff1@email.com', 1, 'Staff');

select * from RentalContract

select * from RentalDetail
select * from RentalContract
select * from Equipment
select * from Payment