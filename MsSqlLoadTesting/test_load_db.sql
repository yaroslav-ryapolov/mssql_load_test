USE [master]
GO

/****** Object:  Database [test_load]    Script Date: 02.03.2024 23:55:22 ******/
CREATE DATABASE [test_load]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'test_load', FILENAME = N'/var/opt/mssql/data/test_load.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'test_load_log', FILENAME = N'/var/opt/mssql/data/test_load_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [test_load].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [test_load] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [test_load] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [test_load] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [test_load] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [test_load] SET ARITHABORT OFF 
GO

ALTER DATABASE [test_load] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [test_load] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [test_load] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [test_load] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [test_load] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [test_load] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [test_load] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [test_load] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [test_load] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [test_load] SET  DISABLE_BROKER 
GO

ALTER DATABASE [test_load] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [test_load] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [test_load] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [test_load] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [test_load] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [test_load] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [test_load] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [test_load] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [test_load] SET  MULTI_USER 
GO

ALTER DATABASE [test_load] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [test_load] SET DB_CHAINING OFF 
GO

ALTER DATABASE [test_load] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [test_load] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [test_load] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [test_load] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO

ALTER DATABASE [test_load] SET QUERY_STORE = ON
GO

ALTER DATABASE [test_load] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO

ALTER DATABASE [test_load] SET  READ_WRITE 
GO


