using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Interfaces;
using TalentoPlus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TalentoPlus.Infrastructure.Services
{
    public class ExcelImportService : IExcelImportService
    {
        private readonly ApplicationDbContext _context;

        public ExcelImportService(ApplicationDbContext context)
        {
            _context = context;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }

        public async Task ImportEmployeesAsync(Stream excelStream)
        {
            using (var package = new ExcelPackage(excelStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var employee = new Employee
                    {
                        // Aquí debes mapear las columnas de tu Excel a las propiedades de Employee
                        // Ejemplo:
                        // Name = worksheet.Cells[row, 1].Text,
                        // Email = worksheet.Cells[row, 2].Text,
                        // ... etc
                    };

                    _context.Employees.Add(employee);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<ImportResult> ImportFromExcel(Stream fileStream)
        {
            var result = new ImportResult();
            
            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var employee = new Employee
                        {
                            // Ajusta estos mapeos según las propiedades de tu entidad Employee
                            // Ejemplo:
                            FirstName = worksheet.Cells[row, 1].Value?.ToString() ?? string.Empty,
                            LastName = worksheet.Cells[row, 2].Value?.ToString() ?? string.Empty,
                            Email = worksheet.Cells[row, 3].Value?.ToString() ?? string.Empty,
                            // Asegúrate de que estas propiedades coincidan con tu modelo Employee
                            // Agrega más mapeos según sea necesario
                        };

                        _context.Employees.Add(employee);
                        result.ImportedCount++;
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Error en fila {row}: {ex.Message}");
                    }
                }

                if (result.ImportedCount > 0)
                {
                    await _context.SaveChangesAsync();
                }
            }

            return result;
        }
    }

    public class ImportResult
    {
        public int ImportedCount { get; set; }
        public List<string> Errors { get; } = new List<string>();
        public bool Success => Errors.Count == 0;
    }
}