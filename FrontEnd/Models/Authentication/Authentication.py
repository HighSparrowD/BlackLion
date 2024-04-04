from Models.ApiModel import ApiModel


class MachineAuth(ApiModel):
    def __init__(self, secret: str):
        self.appSecret: str = secret

class UserAuth(ApiModel):
    def __init__(self, secret: str, userId: int):
        self.appSecret: str = secret
        self.userId: int = userId

class JwtResponse:

    admin = "admin"
    sponsor = "sponsor"
    creator = "creator"
    machine = "machine"

    def __init__(self, accessToken: str, roles: list[str]):
        self.accessToken: str = accessToken
        self.roles: list[str] = roles

    @staticmethod
    def unpack(data_dict: dict[str, str | list[str]]):
        return JwtResponse(data_dict["accessToken"], data_dict["roles"])
