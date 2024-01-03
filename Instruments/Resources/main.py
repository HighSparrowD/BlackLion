from PersonalityResources.Functions import *


set_path_prefixes("./Inputs/", "../../FrontEnd/")
create_registration_resources()
create_prices_resource()
create_settings_resources()
create_currency_setter_resources()
create_shop_resources()

print(f"Resources are loaded")
