from Core.Api import ApiBase
from Models.Authentication.Authentication import creator_role, admin_role
import Models.Admin.Admin as admin_models

class AdminApi:

    def __init__(self, adminId: int):
        self.user_id = adminId
        self.auth_token = None

    def authenticate_admin(self) -> bool:
        jwt_response = ApiBase.authenticate_user(ApiBase.api_key, self.user_id)

        contains_roles = jwt_response.contains_roles(creator_role, admin_role)
        if contains_roles:
            self.auth_token = jwt_response.accessToken

        return contains_roles

    def get_recent_updates(self) -> admin_models.RecentUpdates | None:
        response = ApiBase.create_get_request("api/Admin/updates", authToken=self.auth_token)

        data = response.json()

        return admin_models.RecentUpdates(data)

    #TODO: get_recent_feedbacks

    #TODO: get_all_feedbacks

    def get_recent_reports(self) -> list[admin_models.RecentReports] | None:
        response = ApiBase.create_get_request("api/Admin/reports/recent", authToken=self.auth_token)

        data = response.json()

        return list(admin_models.RecentReports.unpack(data))

    #TODO: get_pending_adventures

    #TODO: get_pending_advertisements
