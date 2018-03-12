export EMAIL_HTML="<html>
  		<head>
			<style> 
				.button { max-width:110px; border:1px solid black; border-radius:6px; font-weight:bold; text-align:center; color:white; padding:4px; cursor:pointer; white-space:nowrap;  } 
				.button > a { color: white; text-decoration: none; }
				.button.success { background-color: #4BC159 }
				.button.unstable { background-color: #FFC05D }
				.button.crash { background-color: #FF8080 }
				.dataTables_info { display:inline; } 
				.dataTables_paginate { float:right; display:inline; }
		 		.table_header { color:#1956b7; } 
		 		.status { color: black; }
                table { border:1px solid black; border-radius:6px; }
                td { padding:10px; }
		 		tr td { vertical-align: middle !important;; }
				tr.success { background-color: #E4FBE4; }
				tr.unstable { background-color: #FBF6E4; }
				tr.crash { background-color: #FBE7E4; }
			</style> 
			<link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css'> 
			<script src='https://code.jquery.com/jquery-3.2.1.min.js' integrity='sha256-hwg4gsxgFZhOsEEamdOYGBf13FyQuiTwlAQgxVSNgt4=' crossorigin='anonymous'></script>
			<script src='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js' integrity='sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa' crossorigin='anonymous'></script>
  		</head>
		<br/><br/>
		<table id='TestResultsTable' class='table dataTable no-footer'>
		<thead><tr class='table_header'><td>Device</td><td>Report</td><td>Result</td></tr></thead>
		<tbody>${DEVICE_HTML_FRAGMENT}</tbody>
		</table> 
	</html>" 

echo "EMAIL_HTML=${EMAIL_HTML}" | sed -e ':a' -e 'N' -e '$!ba' -e 's/\n/ /g' >> results.properties
