import json

import requests
from requests import Response

from Models import ApiModel
from Models.Authentication.Authentication import MachineAuth, JwtResponse, UserAuth

api_address: str
api_key: str
payment_token: str
stripe_key: str
machine_auth: str
api_verify_flag: bool

# Headers
content_type = {"Content-Type": "application/json"}

# Used to for translating Accept-Language header
languages = {
    0: "en-US",
    1: "ru-RU",
    2: "uk-UA"

}

def set_api_address(address):
    global api_address
    api_address = address


def set_api_verify_flag(verify):
    global api_verify_flag
    api_verify_flag = verify


def set_payment_token(token):
    global payment_token
    payment_token = token


def set_stripe_key(key):
    global stripe_key
    stripe_key = key

def set_secret_key(key):
    global api_key
    api_key = key


def authenticate_machine():
    model = MachineAuth(api_key)
    response = create_post_request_with_api_model(model, "api/Authentication/machine")

    if response.status_code == 200:
        token = JwtResponse.unpack(response.json())
        set_machine_jwt(token.accessToken)


def authenticate_user(secret_key: str, userId: int) -> JwtResponse:
    model = UserAuth(secret_key, userId)
    response = create_post_request_with_api_model(model, "api/Authentication/user")

    if response.status_code == 200:
        return JwtResponse.unpack(response.json())


def set_machine_jwt(token):
    global machine_auth
    machine_auth = {"Authorization": f"Bearer {token}"}


def create_get_request(url: str, params: dict[str, any],
                       authToken: str | None = None) -> Response | None:
    try:
        authHeaders = get_auth_header(authToken)
        requests.get(f"{api_address}/{url}", params=params,
                     headers=authHeaders,
                     verify=api_verify_flag)
    except:
        return None


def create_post_request_with_api_model(payload: ApiModel, url: str,
                                       params: dict[str, any] | None = None,
                                       authToken: str | None = None) -> Response | None:
    try:
        data = payload.to_json()

        authHeaders = get_auth_header(authToken)

        headers = content_type
        if authHeaders:
            headers.update(authHeaders)

        response = requests.post(f"{api_address}/{url}", data,
                                 params=params,
                                 headers=headers,
                                 verify=api_verify_flag)
        return response
    except Exception as ex:
        return None


def create_post_request_with_object(data: object, url: str,
                                    params: dict[str, any] | None = None,
                                    authToken: str | None = None) -> Response | None:
    try:
        data = json.dumps(data)

        authHeaders = get_auth_header(authToken)
        headers = content_type
        headers.update(authHeaders)
        response = requests.post(f"{api_address}/{url}", data,
                                 params=params,
                                 headers=headers,
                                 verify=api_verify_flag)
        return response
    except:
        return None


def create_delete_request(url: str, params: dict[str, any],
                          authToken: str | None = None) -> Response | None:
    try:
        authHeaders = get_auth_header(authToken)
        requests.delete(f"{api_address}/{url}", params=params,
                        headers=authHeaders,
                        verify=api_verify_flag)
    except:
        return None

def get_auth_header(authToken: str | None = None):
    if authToken:
        authHeaders = {"Authorization": f"Bearer {authToken}"}
    else:
        authHeaders = machine_auth

    return authHeaders
