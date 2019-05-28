
echo "Gather data from archive directory."
$JSON_HTML_INPUTS = ""
Get-ChildItem "~/Desktop/Thrive_API_Archive" -Filter *.json | 
Foreach-Object {

	echo "Adding JSON to report for $_.FullName."
	$NAME = $_.Name.Replace(".json","")
	$TEMP = Get-Content $_.FullName -Force 
	$JSON_HTML_INPUTS+="<input id='Perf_Data_Build_${NAME}' type='hidden' name='${NAME}' value='${TEMP}' />"
	
}

echo "CD out of archive directory."
cd ..

$HTML = @"
<head>
    <script type='text/javascript' src='.\scripts\jquery.min.js'></script>
    <script type='text/javascript' src='.\scripts\bootstrap.min.js'></script>
    <script type='text/javascript' src='.\scripts\chartsLoader.js'></script>
    <script type='text/javascript' src='.\scripts\jquery.dataTables.min.js'></script>
    <script type='text/javascript' src='.\scripts\api_report.js'></script>
    <link rel='stylesheet' href='.\scripts\bootstrap.min.css'>
</head>
<body>
    {0}
</body>
"@ -f ${JSON_HTML_INPUTS}
echo $HTML | out-file -encoding ASCII ~/Desktop/Thrive_API_Report/report.html
