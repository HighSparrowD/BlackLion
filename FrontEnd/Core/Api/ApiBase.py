import json

import requests
from requests import Response

from Models import ApiModel
from Models.Authentication.Authentication import MachineAuth, JwtResponse

api_address = None
api_verify_flag = False

# Headers
content_type = {"Content-Type": "application/json"}
machine_auth = None

def set_api_address(address):
    global api_address
    api_address = address


def set_api_verify_flag(verify):
    global api_verify_flag
    api_verify_flag = verify


def authenticate_machine(secret_key):
    model = MachineAuth(secret_key)
    response = create_post_request_with_api_model(model, "api/Authentication/machine")

    if response.status_code == 200:
        token = JwtResponse.unpack(response.json())
        set_machine_jwt(token.accessToken)


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
