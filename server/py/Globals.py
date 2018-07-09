
critical_exception = False
test_done = False
test_run_id = ""
heartbeats = ""
test_json = []
ignore_console_printing_to_save_xml_cdata_buffer_size = False #Set during execution so that incoming messages from the client do not get printed to the CLI. This can add up to a lot of data, and this size can cause problems for xmlrunner when it is "Generating Reports..." (including timeouts).
