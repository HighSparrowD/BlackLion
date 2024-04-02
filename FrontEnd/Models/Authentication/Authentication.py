from Models.ApiModel import ApiModel


class MachineAuth(ApiModel):
    def __init__(self, secret: str):
        self.appSecret: str = secret

class JwtResponse:
    def __init__(self, accessToken: str):
        self.accessToken: str = accessToken

    @staticmethod
    def unpack(data_dict: dict[str, str]):
        return JwtResponse(data_dict["accessToken"])
