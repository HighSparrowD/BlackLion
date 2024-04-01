import json

import requests
from requests import Response

from Models import ApiModel

api_address = None
api_verify_flag = False
api_headers = {"Content-Type": "application/json"}


def set_api_address(address):
    global api_address
    api_address = address


def set_api_verify_flag(verify):
    global api_verify_flag
    api_verify_flag = verify


def create_get_request(url: str, params: dict[str, any]) -> Response | None:
    try:
        requests.get(f"{api_address}/{url}", params=params,
                     verify=api_verify_flag)
    except:
        return None


def create_post_request_with_api_model(payload: ApiModel, url: str,
                                       params: dict[str, any] | None = None) -> Response | None:
    try:
        data = payload.to_json()
        response = requests.post(f"{api_address}/{url}", data,
                                 params=params,
                                 headers=api_headers,
                                 verify=api_verify_flag)
        return response
    except Exception as ex:
        return None


def create_post_request_with_object(data: object, url: str,
                                    params: dict[str, any] | None = None) -> Response | None:
    try:
        data = json.dumps(data)
        response = requests.post(f"{api_address}/{url}", data,
                                 params=params,
                                 headers=api_headers,
                                 verify=api_verify_flag)
        return response
    except:
        return None


def create_delete_request(url: str, params: dict[str, any]) -> Response | None:
    try:
        requests.delete(f"{api_address}/{url}", params=params,
                        verify=api_verify_flag)
    except:
        return None
