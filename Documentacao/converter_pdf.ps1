# Abre o relatório no Word, atualiza sumário, listas e numeração e exporta o PDF.
param([Parameter(Mandatory = $true)][string]$Docx, [Parameter(Mandatory = $true)][string]$Pdf)

$word = New-Object -ComObject Word.Application
$word.Visible = $false
$word.DisplayAlerts = 0
try {
    $doc = $word.Documents.Open($Docx)
    $doc.Repaginate()
    # Duas passagens: a primeira ajusta as legendas, a segunda acerta as páginas do sumário
    foreach ($i in 1..2) {
        $doc.Fields.Update() | Out-Null
        foreach ($toc in $doc.TablesOfContents) { $toc.Update() }
        foreach ($tof in $doc.TablesOfFigures) { $tof.Update() }
        $doc.Repaginate()
    }
    $doc.Save()
    $doc.SaveAs2($Pdf, 17)   # 17 = wdFormatPDF
    "Paginas: " + $doc.ComputeStatistics(2)
    $doc.Close($false)
}
finally {
    $word.Quit()
    [System.Runtime.InteropServices.Marshal]::ReleaseComObject($word) | Out-Null
}
