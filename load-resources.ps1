# Load all resources for the API to work properly

# Run API
set-location "WebApi"
./run.ps1

set-location "../../Frontend/Lib/Scripts"
./activate.ps1

set-location "../../../Instruments/Achievement"
python main.py

set-location "../DbTools"
python main.py



deactivate
exit