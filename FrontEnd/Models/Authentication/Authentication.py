from Models.ApiModel import ApiModel

admin_role = "Admin"
sponsor_role = "Sponsor"
creator_role = "Creator"
machine_role = "Machine"

class MachineAuth(ApiModel):
    def __init__(self, secret: str | None):
        self.appSecret: str = secret

class UserAuth(ApiModel):
    def __init__(self, secret: str, userId: int):
        self.appSecret: str = secret
        self.userId: int = userId

class JwtResponse:

    def __init__(self, accessToken: str, roles: list[str]):
        self.accessToken: str = accessToken
        self.roles: list[str] = roles

    def contains_roles(self, *args: str) -> bool:
        for role in args:
            if self.roles.__contains__(role):
                return True

        return False

    @staticmethod
    def unpack(data_dict: dict[str, str | list[str]]):
        return JwtResponse(data_dict["accessToken"], data_dict["roles"])
