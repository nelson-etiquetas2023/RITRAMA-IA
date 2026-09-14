-- ============================================================
-- SCRIPT DE LOGS DE OPERACIONES - Ritrama2025
-- Crea operaciones_log y operaciones_log_detalle si no existen.
-- ============================================================

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='operaciones_log' AND xtype='U')
BEGIN
    CREATE TABLE operaciones_log (
        id BIGINT IDENTITY(1,1) PRIMARY KEY,
        fecha_inicio DATETIME2 NOT NULL DEFAULT GETDATE(),
        fecha_fin DATETIME2 NULL,
        tipo_operacion NVARCHAR(50) NOT NULL,
        descripcion NVARCHAR(500) NOT NULL,
        usuario NVARCHAR(100) NOT NULL DEFAULT '',
        maquina NVARCHAR(100) NOT NULL DEFAULT '',
        ip_address NVARCHAR(50) NOT NULL DEFAULT '',
        exitoso BIT NOT NULL DEFAULT 0,
        resultado NVARCHAR(1000) NOT NULL DEFAULT '',
        detalle_error NVARCHAR(2000) NOT NULL DEFAULT ''
    );
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='operaciones_log_detalle' AND xtype='U')
BEGIN
    CREATE TABLE operaciones_log_detalle (
        id BIGINT IDENTITY(1,1) PRIMARY KEY,
        operacion_id BIGINT NOT NULL,
        fecha DATETIME2 NOT NULL DEFAULT GETDATE(),
        accion NVARCHAR(100) NOT NULL,
        entidad NVARCHAR(100) NOT NULL,
        valor_anterior NVARCHAR(500) NULL,
        valor_nuevo NVARCHAR(500) NULL,
        notas NVARCHAR(1000) NOT NULL DEFAULT '',
        CONSTRAINT FK_log_detalle_operacion FOREIGN KEY (operacion_id) REFERENCES operaciones_log(id)
    );
END
GO

PRINT 'Tablas de operaciones_log creadas correctamente.';
GO