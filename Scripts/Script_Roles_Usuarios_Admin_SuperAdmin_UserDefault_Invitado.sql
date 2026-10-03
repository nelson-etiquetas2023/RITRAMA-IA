-- Roles de usuarios: el catalogo queda en cuatro, que son los que se ofrecen en el
-- combo del modulo de Usuarios (antes era una lista de casillas sin tope).
--
-- Se RENOMBRA en vez de borrar: usuario_roles y role_permisos apuntan a role_id, no al
-- nombre, asi que cambiar el nombre conserva las asignaciones y los permisos que ya
-- estaban dados. Mapeo:
--   Administrador -> Admin
--   Operador      -> User Default
--   Visualizador  -> Invitado
-- Super-Admin es nuevo y nace con los mismos permisos que Admin.
-- Re-ejecutable (idempotente).

-- 1. Los tres existentes, renombrados. El nombre destino se comprueba antes para no
--    chocar con la restricción UNIQUE si alguien ya lo habia creado a mano.
--    Si origen y destino coexisten, NO se renombra: se avisa y se deja el catalogo como
--    está, porque renombrar ahi dejaria dos admins (el viejo con todos los permisos) y el
--    paso 3 copiaria los permisos del rol equivocado.
IF EXISTS (SELECT 1 FROM dbo.roles WHERE nombre = 'Administrador')
   AND EXISTS (SELECT 1 FROM dbo.roles WHERE nombre = 'Admin')
    PRINT 'ATENCION: coexisten Administrador y Admin. Revise el catalogo de roles a mano antes de seguir.';
ELSE IF NOT EXISTS (SELECT 1 FROM dbo.roles WHERE nombre = 'Admin')
    UPDATE dbo.roles SET nombre = 'Admin' WHERE nombre = 'Administrador';
GO

IF NOT EXISTS (SELECT 1 FROM dbo.roles WHERE nombre = 'User Default')
    UPDATE dbo.roles SET nombre = 'User Default' WHERE nombre = 'Operador';
GO

IF NOT EXISTS (SELECT 1 FROM dbo.roles WHERE nombre = 'Invitado')
    UPDATE dbo.roles SET nombre = 'Invitado' WHERE nombre = 'Visualizador';
GO

-- 2. Super-Admin, si todavia no existe.
IF NOT EXISTS (SELECT 1 FROM dbo.roles WHERE nombre = 'Super-Admin')
    INSERT INTO dbo.roles (nombre, descripcion, activo) VALUES ('Super-Admin', 'Acceso total al sistema', 1);
GO

-- 3. Super-Admin hereda los permisos de Admin: si no, nace sin poder hacer nada y
--    ocupa un combo que no sirve. Solo si Admin existe de verdad: sin el, la consulta no
--    matchea, inserta cero filas y Super-Admin queda activo pero vacio — y la app solo
--    filtra por activo, asi que se podria asignar a alguien que no ve nada.
IF EXISTS (SELECT 1 FROM dbo.roles WHERE nombre = 'Admin')
BEGIN
    INSERT INTO dbo.role_permisos (role_id, permiso_id)
    SELECT r.role_id, rp.permiso_id
    FROM dbo.roles r
    CROSS JOIN dbo.roles admin
    JOIN dbo.role_permisos rp ON rp.role_id = admin.role_id
    WHERE r.nombre = 'Super-Admin'
      AND admin.nombre = 'Admin'
      AND NOT EXISTS (
          SELECT 1 FROM dbo.role_permisos x
          WHERE x.role_id = r.role_id AND x.permiso_id = rp.permiso_id
      );
END
ELSE
    PRINT 'ATENCION: no existe el rol Admin, Super-Admin quedo sin permisos. Revise el catalogo de roles a mano.';
GO

-- 4. Los cuatro quedan activos: el combo solo ofrece los activos.
UPDATE dbo.roles SET activo = 1 WHERE nombre IN ('Admin', 'Super-Admin', 'User Default', 'Invitado');
GO