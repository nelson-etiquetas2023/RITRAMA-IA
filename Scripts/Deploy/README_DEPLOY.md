# Deploy Ritrama2025 — paso a paso (T20 / RITRAMASRV01, SQL 2017, AnyDesk)

> Todo lo de BD corre LOCAL en el server. Nada de SSMS por WAN. `1433` solo LAN.

## 0. Pre-requisitos (una vez)
- [ ] `WS 2019/2022` con IP fija, `D:\` datos, `E:\` logs/backups, UPS.
- [ ] SQL 2017 + último `CU31+GDR`, `max server memory` dejando 4 GB al OS.
- [ ] Legacy accesible en la misma instancia (restore del `.bak` anterior).
- [ ] `AnyDesk` unattended + 2FA. Backup FULL de todo antes de empezar.

## 1. Publicar la app (tu PC)
```powershell
dotnet publish Ritrama2025.csproj -c Release -r win-x64 --self-contained true -o .\publish\v1.02
```
Comprime `v1.02` a `zip`, súbelo por `AnyDesk File Transfer` a `D:\Deploy\`.

## 2. Instalar la app (sesión AnyDesk del server)
```powershell
Expand-Archive D:\Deploy\v1.02.zip D:\Apps\Ritrama\v1.02\
robocopy D:\Apps\Ritrama\Current D:\Apps\Ritrama\Backup\v1.01 /MIR
robocopy D:\Apps\Ritrama\v1.02 D:\Apps\Ritrama\Current /MIR /XF appsettings.Production.json
```
Deja `Current\appsettings.Production.json` con `"Ambiente":"Produccion"`,
`RITRAMASRV01/ritrama2026`, `Connect Timeout=15`. Acceso directo al `exe` de `Current`.

## 3. Base de datos (todo con `sqlcmd` local)
```powershell
cd D:\Apps\Ritrama\Current\Scripts\Deploy
# 02 = crea ritrama2026 (si falta) y clona el esquema de RITRAMASQL2017 sin datos.
# (Alternativa manual: SSMS > Generate Scripts > Schema only.)
sqlcmd -S RITRAMASRV01 -E -C -b -v SourceDB="RITRAMASQL2017" -i 02_Clonar_Esquema_Desde_Legacy.sql
.\10_Desplegar_Esquema.ps1 -ServerInstance "RITRAMASRV01" -Database "ritrama2026"
sqlcmd -S RITRAMASRV01 -E -C -b -v Database="ritrama2026" AppLogin="ritrama_app" AppPassword="..." -i 20_Crear_Login_App.sql
sqlcmd -S RITRAMASRV01 -d ritrama2026 -E -C -b -v SourceDB="RITRAMASQL2017" -i 35_Migrar_Maestros_Desde_Legacy.sql
sqlcmd -S RITRAMASRV01 -d ritrama2026 -E -C -b -v SourceDB="RITRAMASQL2017" -i 36_Ajustar_Contadores_Desde_Legacy.sql
sqlcmd -S RITRAMASRV01 -d ritrama2026 -E -C -b -i 30_Verificar_Despliegue.sql
```
`30` debe dar `compat=140`, `en_origen = en_destino`, `sin_interno = 0`.

## 4. Smoke test
Login `admin/admin` → cambia la clave → 1 cliente, 1 proveedor, 1 vendedor,
1 producto (revisa `codigo_interno`), 1 pedido, 1 OC, 1 despacho, 1 etiqueta
TSC, 1 reporte. Da de alta los usuarios reales.

## 5. Rollback
App: `robocopy Backup\v1.01 Current /MIR`. BD: `RESTORE` del FULL previo.
