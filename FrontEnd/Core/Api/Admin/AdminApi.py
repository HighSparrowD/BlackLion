from Core.Api import ApiBase
from Models.Authentication.Authentication import creator_role, admin_role


class AdvertisementApi:

    def __init__(self, adminId: int):
        self.user_id = adminId
        self.auth_token = None

    def authenticate_sponsor(self) -> bool:
        jwt_response = ApiBase.authenticate_user(ApiBase.api_key, self.user_id)

        contains_roles = jwt_response.contains_roles(creator_role. admin_role)
        if contains_roles:
            self.auth_token = jwt_response.accessToken

        return contains_roles


