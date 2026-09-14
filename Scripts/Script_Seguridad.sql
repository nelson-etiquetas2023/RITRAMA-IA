-- ============================================================
-- SCRIPT DE SEGURIDAD - Ritrama2025
-- Crear tablas: usuarios, roles, permisos, relaciones, auditoria
-- ============================================================

-- 1. TABLA USUARIOS
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='usuarios' AND xtype='U')
BEGIN
    CREATE TABLE usuarios (
        user_id INT IDENTITY(1,1) PRIMARY KEY,
        username NVARCHAR(50) UNIQUE NOT NULL,
        password_hash NVARCHAR(255) NOT NULL,
        nombre_completo NVARCHAR(150) NOT NULL,
        email NVARCHAR(100) NULL,
        activo BIT DEFAULT 1,
        primer_login BIT DEFAULT 1,
        fecha_creacion DATETIME DEFAULT GETDATE(),
        ultimo_login DATETIME NULL
    );
END
GO

-- 2. TABLA ROLES
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='roles' AND xtype='U')
BEGIN
    CREATE TABLE roles (
        role_id INT IDENTITY(1,1) PRIMARY KEY,
        nombre NVARCHAR(50) UNIQUE NOT NULL,
        descripcion NVARCHAR(200) NULL,
        activo BIT DEFAULT 1
    );
END
GO

-- 3. TABLA PERMISOS
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='permisos' AND xtype='U')
BEGIN
    CREATE TABLE permisos (
        permiso_id INT IDENTITY(1,1) PRIMARY KEY,
        modulo NVARCHAR(50) NOT NULL,
        accion NVARCHAR(50) NOT NULL,
        descripcion NVARCHAR(200) NULL,
        CONSTRAINT UQ_modulo_accion UNIQUE (modulo, accion)
    );
END
GO

-- 4. TABLA USUARIO_ROLES (asignacion)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='usuario_roles' AND xtype='U')
BEGIN
    CREATE TABLE usuario_roles (
        user_id INT NOT NULL,
        role_id INT NOT NULL,
        PRIMARY KEY (user_id, role_id),
        FOREIGN KEY (user_id) REFERENCES usuarios(user_id),
        FOREIGN KEY (role_id) REFERENCES roles(role_id)
    );
END
GO

-- 5. TABLA ROLE_PERMISOS (asignacion)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='role_permisos' AND xtype='U')
BEGIN
    CREATE TABLE role_permisos (
        role_id INT NOT NULL,
        permiso_id INT NOT NULL,
        PRIMARY KEY (role_id, permiso_id),
        FOREIGN KEY (role_id) REFERENCES roles(role_id),
        FOREIGN KEY (permiso_id) REFERENCES permisos(permiso_id)
    );
END
GO

-- 6. TABLA AUDITORIA_LOGIN
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='auditoria_login' AND xtype='U')
BEGIN
    CREATE TABLE auditoria_login (
        log_id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NULL,
        fecha DATETIME DEFAULT GETDATE(),
        exitoso BIT,
        ip_address NVARCHAR(50) NULL,
        FOREIGN KEY (user_id) REFERENCES usuarios(user_id)
    );
END
GO

-- ============================================================
-- DATOS INICIALES
-- ============================================================

-- Roles base
IF NOT EXISTS (SELECT 1 FROM roles WHERE nombre = 'Administrador')
    INSERT INTO roles (nombre, descripcion) VALUES ('Administrador', 'Acceso total al sistema');

IF NOT EXISTS (SELECT 1 FROM roles WHERE nombre = 'Operador')
    INSERT INTO roles (nombre, descripcion) VALUES ('Operador', 'Acceso limitado a modulos asignados');

IF NOT EXISTS (SELECT 1 FROM roles WHERE nombre = 'Visualizador')
    INSERT INTO roles (nombre, descripcion) VALUES ('Visualizador', 'Solo consulta, sin edicion');

-- Permisos por modulo
DECLARE @modulos TABLE (modulo NVARCHAR(50), accion NVARCHAR(50), descripcion NVARCHAR(200));

INSERT INTO @modulos VALUES
('Produccion', 'Ver', 'Ver ordenes de corte'),
('Produccion', 'Crear', 'Crear ordenes de corte'),
('Produccion', 'Editar', 'Editar ordenes de corte'),
('Produccion', 'Eliminar', 'Eliminar ordenes de corte'),
('Despacho', 'Ver', 'Ver despachos'),
('Despacho', 'Crear', 'Crear despachos'),
('Despacho', 'Editar', 'Editar despachos'),
('Despacho', 'Eliminar', 'Eliminar despachos'),
('Inventario', 'Ver', 'Ver inventario'),
('Inventario', 'Crear', 'Crear registros de inventario'),
('Inventario', 'Editar', 'Editar registros de inventario'),
('Recepciones', 'Ver', 'Ver recepciones de materia prima'),
('Recepciones', 'Crear', 'Crear recepciones'),
('Recepciones', 'Editar', 'Editar recepciones'),
('Productos', 'Ver', 'Ver productos'),
('Productos', 'Crear', 'Crear productos'),
('Productos', 'Editar', 'Editar productos'),
('Productos', 'Eliminar', 'Eliminar productos'),
('Clientes', 'Ver', 'Ver clientes'),
('Clientes', 'Crear', 'Crear clientes'),
('Clientes', 'Editar', 'Editar clientes'),
('Clientes', 'Eliminar', 'Eliminar clientes'),
('Pedidos', 'Ver', 'Ver pedidos'),
('Pedidos', 'Crear', 'Crear pedidos'),
('Pedidos', 'Editar', 'Editar pedidos'),
('Pedidos', 'Eliminar', 'Eliminar pedidos'),
('Reportes', 'Ver', 'Ver reportes'),
('Etiquetas', 'Ver', 'Ver etiquetas'),
('Etiquetas', 'Crear', 'Crear etiquetas'),
('Etiquetas', 'Editar', 'Editar etiquetas'),
('Proveedores', 'Ver', 'Ver proveedores'),
('Proveedores', 'Crear', 'Crear proveedores'),
('Proveedores', 'Editar', 'Editar proveedores'),
('Usuarios', 'Ver', 'Ver usuarios del sistema'),
('Usuarios', 'Crear', 'Crear usuarios'),
('Usuarios', 'Editar', 'Editar usuarios'),
('Usuarios', 'Eliminar', 'Eliminar usuarios'),
('Roles', 'Ver', 'Ver roles'),
('Roles', 'Crear', 'Crear roles'),
('Roles', 'Editar', 'Editar roles'),
('Roles', 'Eliminar', 'Eliminar roles');

INSERT INTO permisos (modulo, accion, descripcion)
SELECT m.modulo, m.accion, m.descripcion
FROM @modulos m
WHERE NOT EXISTS (
    SELECT 1 FROM permisos p WHERE p.modulo = m.modulo AND p.accion = m.accion
);
GO

-- Usuario admin inicial (password: admin)
-- Hash BCrypt generado en codigo. Si ya existe, no duplicar.
IF NOT EXISTS (SELECT 1 FROM usuarios WHERE username = 'admin')
BEGIN
    -- Hash de 'admin' con BCrypt
    INSERT INTO usuarios (username, password_hash, nombre_completo, email, activo, primer_login)
    VALUES ('admin', '$2a$11$e0BB0gbczULd8V9g.2QdmuGwbqN0a724OsfLsztc4WWzqDe3.Om0q', 'Administrador General', 'admin@ritrama.com', 1, 0);

    -- Asignar rol Administrador
    INSERT INTO usuario_roles (user_id, role_id)
    SELECT u.user_id, r.role_id
    FROM usuarios u, roles r
    WHERE u.username = 'admin' AND r.nombre = 'Administrador';

    -- Asignar TODOS los permisos al rol Administrador
    INSERT INTO role_permisos (role_id, permiso_id)
    SELECT r.role_id, p.permiso_id
    FROM roles r, permisos p
    WHERE r.nombre = 'Administrador';
END
GO

PRINT 'Script de seguridad ejecutado correctamente.';
GO
