# PlataformaDigital Backend

Backend modular para un HUB administrativo interno construido con **DDD + Clean Architecture + CQRS**.

## Arquitectura
- `src/PlataformaDigital.Api`: host principal, middleware transversal, Swagger, auth y health checks.
- `src/PlataformaDigital.SharedKernel`: abstracciones y tipos base de dominio.
- `src/PlataformaDigital.BuildingBlocks`: contratos transversales (result, paginación, servicios externos).
- `src/PlataformaDigital.Modules.*`: módulos funcionales (Projects, Treasury, People) separados por Domain, Application, Infrastructure y Api.
- `src/PlataformaDigital.Workers`: workers programados.
- `tests/*`: pruebas unitarias e integración.

## Cómo ejecutar
1. Instalar .NET LTS (8 o superior).
2. Configurar `ConnectionStrings:DefaultConnection` en `src/PlataformaDigital.Api/appsettings.Development.json`.
3. Restaurar y ejecutar:
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project src/PlataformaDigital.Api
   ```
4. Abrir Swagger en `/swagger`.

## Cómo agregar módulos
1. Crear cuatro proyectos: `Domain`, `Application`, `Infrastructure`, `Api`.
2. Referenciar `SharedKernel` y `BuildingBlocks` según capa.
3. Registrar el módulo en `PlataformaDigital.Api/Program.cs`.
4. Definir endpoints en `Modules.<X>.Api/DependencyInjection.cs` usando MediatR.

## Cómo agregar casos de uso
1. Crear `Command/Query` en Application.
2. Crear `Handler` con MediatR y validación FluentValidation.
3. Exponer endpoint en Api del módulo.
4. Añadir pruebas unitarias de reglas de dominio y handler.

## Decisiones de diseño
- Modular monolith preparado para extraer microservicios por módulo.
- CQRS con MediatR para desacoplar endpoints y casos de uso.
- Result Pattern y paginación estándar para contratos consistentes.
- Servicios externos desacoplados por interfaces (`IEmailService`, `IExcelExportService`, etc.).

## Pendientes para producción
- Endurecer seguridad JWT con políticas por permisos.
- Integrar proveedores reales (correo, excel, storage).
- Observabilidad (OpenTelemetry, métricas, trazas distribuidas).
- Estrategia completa de migraciones y CI/CD.
