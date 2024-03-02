USE [master]
GO

/****** Object:  Database [test_load_in_memory]    Script Date: 02.03.2024 23:57:33 ******/
CREATE DATABASE [test_load_in_memory]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'test_load_in_memory', FILENAME = N'/var/opt/mssql/data/test_load_in_memory.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB ), 
 FILEGROUP [IN_MEMORY] CONTAINS MEMORY_OPTIMIZED_DATA  DEFAULT
( NAME = N'mem_file', FILENAME = N'/var/opt/mssql/data/test_load_in_memory_mem' , MAXSIZE = UNLIMITED)
 LOG ON 
( NAME = N'test_load_in_memory_log', FILENAME = N'/var/opt/mssql/data/test_load_in_memory_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [test_load_in_memory].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [test_load_in_memory] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [test_load_in_memory] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [test_load_in_memory] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [test_load_in_memory] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [test_load_in_memory] SET ARITHABORT OFF 
GO

ALTER DATABASE [test_load_in_memory] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [test_load_in_memory] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [test_load_in_memory] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [test_load_in_memory] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [test_load_in_memory] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [test_load_in_memory] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [test_load_in_memory] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [test_load_in_memory] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [test_load_in_memory] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [test_load_in_memory] SET  DISABLE_BROKER 
GO

ALTER DATABASE [test_load_in_memory] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [test_load_in_memory] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [test_load_in_memory] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [test_load_in_memory] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [test_load_in_memory] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [test_load_in_memory] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [test_load_in_memory] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [test_load_in_memory] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [test_load_in_memory] SET  MULTI_USER 
GO

ALTER DATABASE [test_load_in_memory] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [test_load_in_memory] SET DB_CHAINING OFF 
GO

ALTER DATABASE [test_load_in_memory] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [test_load_in_memory] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [test_load_in_memory] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [test_load_in_memory] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO

ALTER DATABASE [test_load_in_memory] SET QUERY_STORE = ON
GO

ALTER DATABASE [test_load_in_memory] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO

ALTER DATABASE [test_load_in_memory] SET  READ_WRITE 
GO

ALTER DATABASE [test_load_in_memory] SET MEMORY_OPTIMIZED_ELEVATE_TO_SNAPSHOT = ON
GO