# WinMaintenanceTool

Aplicação desktop em **C# 14 + .NET 10** (WPF + WPF-UI 4.1) para executar comandos de manutenção **SFC** e **DISM** em uma interface moderna para Windows 11.

## Estrutura

- `src/WinMaintenanceTool/Assets` → ícones e imagens.
- `src/WinMaintenanceTool/Models` → modelos do domínio.
- `src/WinMaintenanceTool/Services` → serviços (execução de comandos, configuração e localização).
- `src/WinMaintenanceTool/ViewModels` → lógica MVVM.
- `src/WinMaintenanceTool/Views` → interface XAML.
- `src/WinMaintenanceTool/Resources` → arquivos `.resx` (English e Português do Brasil).
- `build.ps1` → build portátil self-contained/single-file para `Build/`.

## Build

Execute no PowerShell (Windows):

```powershell
./build.ps1
```

Saída final: pasta `Build/`.
