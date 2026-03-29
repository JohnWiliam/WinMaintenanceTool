<#
.SYNOPSIS
    Ferramenta de Manutenção do Windows com TUI Avançada.
.DESCRIPTION
    Script PowerShell 7.2+ utilizando $PSStyle e renderização em bloco geométrico perfeito.
    Desenvolvido por John Wiliam.
#>

#requires -Version 7.2
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not $IsWindows) {
    throw 'Este script foi projetado para Windows.'
}

# =============================================================================
# 1. HOOK DE AMBIENTE E AUTO-ELEVAÇÃO
# =============================================================================

$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
$isWT = $env:WT_SESSION -ne $null

if (-not $isAdmin -or -not $isWT) {
    $scriptPath = $MyInvocation.MyCommand.Path
    try {
        Start-Process "wt.exe" -ArgumentList "pwsh.exe -NoProfile -File `"$scriptPath`"" -Verb RunAs
        Exit
    } catch {
        Write-Host "Erro ao iniciar o Windows Terminal." -ForegroundColor Red
        Pause
        Exit
    }
}

# =============================================================================
# 2. CONFIGURAÇÕES NATIVAS E TEMA
# =============================================================================

try { $PSNativeCommandUseErrorActionPreference = $false } catch {}
try { [Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false) } catch {}

$Host.UI.RawUI.WindowTitle = 'Ferramenta de manutenção do Windows'

if ($Host.UI.SupportsVirtualTerminal) {
    $PSStyle.OutputRendering = 'ANSI'
    $PSStyle.Progress.View = 'Minimal'
}

# Paleta de cores exata do seu projeto!
$Theme = [ordered]@{
    Title     = "$($PSStyle.Bold)$($PSStyle.Foreground.BrightCyan)"
    Subtitle  = "$($PSStyle.Bold)$($PSStyle.Foreground.BrightYellow)"
    Border    = "$($PSStyle.Bold)$($PSStyle.Foreground.BrightBlue)"
    Option    = "$($PSStyle.Bold)$($PSStyle.Foreground.BrightGreen)"
    Desc      = "$($PSStyle.Foreground.BrightMagenta)"
    Prompt    = "$($PSStyle.Bold)$($PSStyle.Foreground.BrightYellow)"
    Info      = "$($PSStyle.Foreground.BrightCyan)"
    Success   = "$($PSStyle.Bold)$($PSStyle.Foreground.BrightGreen)"
    Warning   = "$($PSStyle.Bold)$($PSStyle.Foreground.BrightRed)"
    Reset     = "$($PSStyle.Reset)"
}

# =============================================================================
# 3. MOTOR DA TUI (RENDERIZAÇÃO DE COMPONENTES)
# =============================================================================

function Wrap-Text {
    param([string]$Text, [int]$MaxWidth)
    if ([string]::IsNullOrWhiteSpace($Text)) { return @("") }
    $words = $Text.Split(' ')
    $lines = @()
    $currentLine = ""
    foreach ($word in $words) {
        if (($currentLine.Length + $word.Length + 1) -gt $MaxWidth) {
            if ($currentLine.Length -gt 0) { $lines += $currentLine; $currentLine = $word }
            else { $lines += $word.Substring(0, $MaxWidth); $currentLine = "" }
        } else {
            if ($currentLine.Length -eq 0) { $currentLine = $word }
            else { $currentLine += " " + $word }
        }
    }
    if ($currentLine.Length -gt 0) { $lines += $currentLine }
    return $lines
}

# Calcula o Padding ignorando o "peso invisível" das cores ANSI e de Variações de Emojis
function Pad-Text {
    param([string]$Text, [int]$Width, [string]$Align = 'Left')
    
    # Remove as cores ANSI invisíveis do cálculo
    $plainText = $Text -replace '\x1B\[[0-9;]*m', ''
    
    # FIX: Converte o caractere invisível para [string] ANTES do Replace
    $varSelector = [string][char]0xFE0F
    $zeroWidthJoiner = [string][char]0x200D
    
    $plainText = $plainText.Replace($varSelector, '').Replace($zeroWidthJoiner, '')
    
    $len = $plainText.Length
    
    if ($len -ge $Width) { return $Text }
    
    if ($Align -eq 'Center') {
        $left = [math]::Floor(($Width - $len) / 2)
        $right = $Width - $len - $left
        return (" " * $left) + $Text + (" " * $right)
    } else {
        return $Text + (" " * ($Width - $len))
    }
}

function Show-MenuScreen {
    param(
        [string]$Title,
        [string]$Subtitle,
        [array]$Options
    )
    Clear-Host
    
    $winWidth = $Host.UI.RawUI.WindowSize.Width
    if ($winWidth -lt 60) { $winWidth = 120 }
    $boxWidth = [math]::Min(110, $winWidth - 10)
    $innerWidth = $boxWidth - 4
    
    $leftMargin = [math]::Max([math]::Floor(($winWidth - $boxWidth) / 2), 0)
    $margin = " " * $leftMargin

    $TL="╔"; $TR="╗"; $BL="╚"; $BR="╝"; $H="═"; $V="║"; $L="╠"; $R="╣"
    
    Write-Host ""
    Write-Host ($margin + $Theme.Border + $TL + ($H * ($boxWidth - 2)) + $TR + $Theme.Reset)

    # Renderiza o Título centralizado e corrigido
    $titleLines = Wrap-Text $Title $innerWidth
    foreach ($line in $titleLines) {
        $padded = Pad-Text $line $innerWidth 'Center'
        Write-Host ($margin + $Theme.Border + $V + " " + $Theme.Title + $padded + $Theme.Border + " " + $V + $Theme.Reset)
    }

    Write-Host ($margin + $Theme.Border + $L + ($H * ($boxWidth - 2)) + $R + $Theme.Reset)

    # Renderiza o Subtítulo
    $subLines = Wrap-Text $Subtitle $innerWidth
    foreach ($line in $subLines) {
        $padded = Pad-Text $line $innerWidth 'Center'
        Write-Host ($margin + $Theme.Border + $V + " " + $Theme.Subtitle + $padded + $Theme.Border + " " + $V + $Theme.Reset)
    }

    Write-Host ($margin + $Theme.Border + $L + ($H * ($boxWidth - 2)) + $R + $Theme.Reset)

    $blank = Pad-Text "" $innerWidth
    Write-Host ($margin + $Theme.Border + $V + " " + $blank + " " + $V + $Theme.Reset)
    
    # Renderiza as Opções
    foreach ($opt in $Options) {
        $optTitle = "$($Theme.Prompt)$($opt.Key)) $($Theme.Option)$($opt.Name)$($Theme.Reset)"
        $paddedOptTitle = Pad-Text $optTitle $innerWidth 'Left'
        
        Write-Host ($margin + $Theme.Border + $V + " " + $paddedOptTitle + $Theme.Border + " " + $V + $Theme.Reset)

        if ($opt.Desc) {
            $descLines = Wrap-Text "   $($opt.Desc)" $innerWidth
            foreach ($line in $descLines) {
                $paddedDesc = Pad-Text $line $innerWidth 'Left'
                Write-Host ($margin + $Theme.Border + $V + " " + $Theme.Desc + $paddedDesc + $Theme.Border + " " + $V + $Theme.Reset)
            }
        }
        Write-Host ($margin + $Theme.Border + $V + " " + $blank + " " + $V + $Theme.Reset)
    }

    Write-Host ($margin + $Theme.Border + $BL + ($H * ($boxWidth - 2)) + $BR + $Theme.Reset)
    Write-Host ""
}

# =============================================================================
# 4. EXECUTOR DE TAREFAS
# =============================================================================

function Invoke-Task {
    param ([string]$Command)
    Clear-Host
    Write-Host "`n$($Theme.Info)Aguarde, executando o comando...$($Theme.Reset)"
    Write-Host "$($Theme.Subtitle)> $Command$($Theme.Reset)`n"
    Write-Host "$($Theme.Border)======================================================$($Theme.Reset)"
    
    Invoke-Expression $Command

    Write-Host "`n$($Theme.Border)======================================================$($Theme.Reset)"
    Write-Host "$($Theme.Success)Processo concluído.$($Theme.Reset)"
    
    while ($true) {
        Write-Host ""
        Write-Host -NoNewline "$($Theme.Prompt)(v) Voltar > $($Theme.Reset)"
        $inputKey = Read-Host
        if ($inputKey.Trim().ToLower() -eq 'v') { break }
    }
}

# =============================================================================
# 5. PÁGINAS DO SISTEMA
# =============================================================================

function Show-MenuSFC {
    $running = $true
    while ($running) {
        $opts = @(
            @{ Key="1"; Name="SFC /scannow"; Desc="Descrição: Examina todos os arquivos de sistema protegidos e repara os arquivos com problema quando possível." }
            @{ Key="2"; Name="SFC /verifyonly"; Desc="Descrição: Examina todos os arquivos protegidos, mas não faz reparo." }
            @{ Key="V"; Name="Voltar"; Desc="" }
        )
        
        Show-MenuScreen `
            -Title "🛠️ SFC (System File Checker)" `
            -Subtitle "Descrição: O SFC (System File Checker) é uma ferramenta nativa do Windows que verifica a integridade e repara arquivos corrompidos ou ausentes do sistema." `
            -Options $opts

        Write-Host -NoNewline "$($Theme.Prompt)Selecione uma opção > $($Theme.Reset)"
        $choice = Read-Host
        
        switch ($choice.Trim().ToLower()) {
            '1' { Invoke-Task "sfc /scannow" }
            '2' { Invoke-Task "sfc /verifyonly" }
            'v' { $running = $false }
            default { Write-Host "$($Theme.Warning)Opção inválida.$($Theme.Reset)"; Start-Sleep -Seconds 1 }
        }
    }
}

function Show-MenuDISM {
    $running = $true
    while ($running) {
        $opts = @(
            @{ Key="1"; Name="DISM /Online /Cleanup-Image /CheckHealth"; Desc="Descrição: Verifica se a imagem foi marcada como corrompida e se o dano é reparável." }
            @{ Key="2"; Name="DISM /Online /Cleanup-Image /ScanHealth"; Desc="Descrição: Faz uma varredura mais profunda em busca de corrupção no component store." }
            @{ Key="3"; Name="DISM /Online /Cleanup-Image /RestoreHealth"; Desc="Descrição: Detecta e repara automaticamente corrupção na imagem/component store." }
            @{ Key="4"; Name="DISM /Online /Cleanup-Image /AnalyzeComponentStore"; Desc="Descrição: Gera análise do estado e do tamanho do component store." }
            @{ Key="5"; Name="DISM /Online /Cleanup-Image /StartComponentCleanup"; Desc="Descrição: Remove componentes substituídos e reduz o tamanho do component store." }
            @{ Key="6"; Name="DISM /Online /Cleanup-Image /StartComponentCleanup /ResetBase"; Desc="Descrição: Consolida a base dos componentes e impede a desinstalação posterior dos updates já integrados." }
            @{ Key="V"; Name="Voltar"; Desc="" }
        )
        
        Show-MenuScreen `
            -Title "🛠️ DISM (Gerenciamento e Manutenção de Imagens)" `
            -Subtitle "Descrição: O DISM é uma ferramenta de linha de comando para reparar imagens corrompidas do sistema, arquivos do Update e componentes." `
            -Options $opts

        Write-Host -NoNewline "$($Theme.Prompt)Selecione uma opção > $($Theme.Reset)"
        $choice = Read-Host
        
        switch ($choice.Trim().ToLower()) {
            '1' { Invoke-Task "DISM /Online /Cleanup-Image /CheckHealth" }
            '2' { Invoke-Task "DISM /Online /Cleanup-Image /ScanHealth" }
            '3' { Invoke-Task "DISM /Online /Cleanup-Image /RestoreHealth" }
            '4' { Invoke-Task "DISM /Online /Cleanup-Image /AnalyzeComponentStore" }
            '5' { Invoke-Task "DISM /Online /Cleanup-Image /StartComponentCleanup" }
            '6' { Invoke-Task "DISM /Online /Cleanup-Image /StartComponentCleanup /ResetBase" }
            'v' { $running = $false }
            default { Write-Host "$($Theme.Warning)Opção inválida.$($Theme.Reset)"; Start-Sleep -Seconds 1 }
        }
    }
}

function Show-MainMenu {
    $running = $true
    while ($running) {
        $opts = @(
            @{ Key="1"; Name="SFC"; Desc="Descrição: Verifica a integridade dos arquivos de sistema protegidos e substitui arquivos corrompidos." }
            @{ Key="2"; Name="DISM"; Desc="Descrição: Verifica, mantém e repara a imagem do Windows, incluindo o armazenamento de componentes." }
            @{ Key="S"; Name="Sair do sistema"; Desc="" }
        )
        
        Show-MenuScreen `
            -Title "🛠️ Ferramenta de manutenção do Windows" `
            -Subtitle "Desenvolvido por John Wiliam" `
            -Options $opts

        Write-Host -NoNewline "$($Theme.Prompt)Selecione uma opção > $($Theme.Reset)"
        $choice = Read-Host
        
        switch ($choice.Trim().ToLower()) {
            '1' { Show-MenuSFC }
            '2' { Show-MenuDISM }
            's' { $running = $false }
            default { Write-Host "$($Theme.Warning)Opção inválida.$($Theme.Reset)"; Start-Sleep -Seconds 1 }
        }
    }
}

# =============================================================================
# 6. INICIALIZAÇÃO E ENCERRAMENTO
# =============================================================================

Show-MainMenu

Clear-Host
$winWidth = $Host.UI.RawUI.WindowSize.Width
if ($winWidth -lt 50) { $winWidth = 120 }
$margin = " " * [math]::Max([math]::Floor(($winWidth - 46) / 2), 0)

Write-Host "`n"
Write-Host ($margin + $Theme.Border + "╔══════════════════════════════════════════╗" + $Theme.Reset)
Write-Host ($margin + $Theme.Border + "║ " + $Theme.Success + (Pad-Text "Ferramenta encerrada. Até logo!" 40 'Center') + $Theme.Border + " ║" + $Theme.Reset)
Write-Host ($margin + $Theme.Border + "╚══════════════════════════════════════════╝" + $Theme.Reset)
Write-Host "`n"

Start-Sleep -Seconds 2
Exit