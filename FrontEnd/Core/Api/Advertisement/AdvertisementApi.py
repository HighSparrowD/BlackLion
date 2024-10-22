import calendar
import datetime
from typing import Union

from requests import Response

import Models.Generic.Generic as generic_models
import Models.Advertisement.Advertisement as advertisement_models
from Core.Api import ApiBase
from Models.Authentication.Authentication import sponsor_role


class AdvertisementApi:

    def __init__(self, sponsorId: int):
        self.user_id = sponsorId
        self.auth_token = None

    def authenticate_sponsor(self) -> bool:
        jwt_response = ApiBase.authenticate_user(ApiBase.api_key, self.user_id)

        contains_roles = jwt_response.roles.__contains__(sponsor_role)
        if contains_roles:
            self.auth_token = jwt_response.accessToken

        return contains_roles

    def add_advertisement(self, payload: advertisement_models.AdvertisementNew) -> Union[Response, None]:
        try:
            response = ApiBase.create_post_request_with_api_model("advertisement", payload, authToken=self.auth_token)
            return response

        except:
            return None

    def update_advertisement(self, payload: advertisement_models.AdvertisementUpdate) -> Union[Response, None]:
        try:
            response = ApiBase.create_put_request("advertisement", payload, authToken=self.auth_token)
            return response

        except:
            return None

    def get_advertisement_list(self, userId) -> Union[list[advertisement_models.AdvertisementItem], None]:
        try:
            response = ApiBase.create_get_request(f"advertisement-list/{userId}", authToken=self.auth_token)
            advertisements = response.json()

            return [advertisement_models.AdvertisementItem(advertisement) for advertisement in advertisements]
        except:
            return

    def get_advertisement_info(self, adId) -> Union[advertisement_models.Advertisement, None]:
        response = ApiBase.create_get_request(f"advertisement/{adId}", authToken=self.auth_token)
        advertisement = response.json()

        return advertisement_models.Advertisement(advertisement)

    def delete_advertisement(self, advertisementId) -> Union[Response, None]:
        try:
            response = ApiBase.create_delete_request(f"advertisement/{advertisementId}", authToken=self.auth_token)
            return response
        except:
            return

    def get_all_advertisement_priorities(self) -> Union[list[generic_models.LocalizedEnum], None]:
        try:
            response = ApiBase.create_get_request(f"priorities", authToken=self.auth_token)
            priorities = response.json()

            return [generic_models.LocalizedEnum(priority) for priority in priorities]
        except:
            return

    def get_advertisement_economy_monthly_statistics(self, user_id, advertisement_id=None) \
            -> list[advertisement_models.StatisticsEconomy] | None:
        try:
            payload = self.create_statistics_request_model()
            params = {"advertisementId": advertisement_id}

            response = ApiBase.create_post_request_with_api_model(f"statistics/economy/{user_id}", payload,
                                                                  params, authToken=self.auth_token)
            stats = response.json()

            return advertisement_models.StatisticsEconomy.unpack(stats)

        except:
            return None

    def get_advertisement_engagement_monthly_statistics(self, user_id, advertisement_id=None) \
            -> list[advertisement_models.StatisticsEngagement] | None:
        try:
            payload = self.create_statistics_request_model()
            params = {"advertisementId": advertisement_id}

            response = ApiBase.create_post_request_with_api_model(f"statistics/engagement/{user_id}", payload,
                                                                  params, authToken=self.auth_token)
            stats = response.json()

            return advertisement_models.StatisticsEngagement.unpack(stats)

        except:
            return None

    def get_overall_economy_monthly_statistics(self, user_id) -> list[advertisement_models.StatisticsEconomy] | None:
        try:
            payload = self.create_statistics_request_model()

            response = ApiBase.create_post_request_with_api_model(f"statistics/economy/{user_id}", payload,
                                                                  authToken=self.auth_token)
            stats = response.json()

            return advertisement_models.StatisticsEconomy.unpack(stats)

        except:
            return None

    def get_overall_engagement_monthly_statistics(self, user_id) -> list[advertisement_models.StatisticsEngagement] | None:
        try:
            payload = self.create_statistics_request_model()

            response = ApiBase.create_post_request_with_api_model(f"statistics/engagement/{user_id}", payload,
                                                                  authToken=self.auth_token)
            stats = response.json()

            return advertisement_models.StatisticsEngagement.unpack(stats)

        except:
            return None

    @staticmethod
    def create_statistics_request_model() -> advertisement_models.StatisticsGet:
        model = advertisement_models.StatisticsGet()

        now = datetime.datetime.now()
        max_days = calendar.monthrange(now.year, now.month)[1]

        model.from_ = datetime.datetime(now.year, now.month, 1)
        model.to = datetime.datetime(now.year, now.month, max_days)

        return model
