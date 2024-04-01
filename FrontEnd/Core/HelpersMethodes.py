import calendar
import datetime
import json
from typing import Union

import requests
from requests import Response

import Models.Advertisement.Advertisement as advertisement_models
import Models.User.User as user_models
import Models.Generic.Generic as generic_models
from Core.Api import ApiBase

# Used to for translating Accept-Language header
languages = {
    0: "en-US",
    1: "ru-RU",
    2: "uk-UA"
}

payment_token = None
stripe_key = None

points = "Points"


def set_payment_token(token):
    global payment_token
    payment_token = token


def set_stripe_key(key):
    global stripe_key
    stripe_key = key


def format_tags(tag_string: str) -> str:
    tags = tag_string.lower().replace("_", "").replace("-", "").replace(" ", "")
    return tags


def check_user_exists(userId):
    return bool(json.loads(requests.get(f"{ApiBase.api_address}/CheckUserExists/{userId}", verify=False).text))


def check_user_is_sponsor(userId):
    return bool(json.loads(
        requests.get(f"{ApiBase.api_address}/CheckUserIsSponsor/{userId}", verify=False).text))


def check_user_is_awaiting(userId):
    return bool(json.loads(
        requests.get(f"{ApiBase.api_address}/CheckUserIsAwaiting/{userId}", verify=False).text))


def check_user_is_awaiting_by_username(username):
    return bool(json.loads(
        requests.get(f"{ApiBase.api_address}/CheckUserIsAwaitingByUsername/{username}", verify=False).text))


def check_sponsor_is_maxed(userId):
    return bool(json.loads(
        requests.get(f"{ApiBase.api_address}/CheckSponsorIsMaxed/{userId}", verify=False).text))


def check_user_keyword_is_correct(userId, keyword):
    return bool(json.loads(
        requests.get(f"{ApiBase.api_address}/CheckUserKeyWordIsCorrect/{userId}/{keyword}", verify=False).text))


def check_user_is_admin(userId):
    return bool(
        json.loads(requests.get(f"{ApiBase.api_address}/CheckUserIsAdmin/{userId}", verify=False).text))


def check_user_is_postponed(userId):
    return bool(json.loads(
        requests.get(f"{ApiBase.api_address}/CheckSponsorIsPostponed/{userId}", verify=False).text))


def check_user_is_registered(userId):
    return bool(json.loads(
        requests.get(f"{ApiBase.api_address}/CheckUserIsRegistered/{userId}", verify=False).text))


def check_user_has_visited_section(userId, sectionId):
    try:
        return bool(json.loads(
            requests.get(f"{ApiBase.api_address}/CheckUserHasVisitedSection/{userId}/{sectionId}",
                         verify=False).text))
    except:
        return None


def check_user_is_banned(userId):
    return bool(json.loads(
        requests.get(f"{ApiBase.api_address}/CheckUserIsBanned/{userId}", verify=False).text))


def check_user_is_busy(userId):
    try:
        return bool(json.loads(
            requests.get(f"{ApiBase.api_address}/CheckUserIsBusy/{userId}", verify=False).text))
    except:
        return None


def switch_user_busy_status(userId, sectionId):
    try:
        return json.loads(
            requests.get(f"{ApiBase.api_address}/switch-busy-status/{userId}/{sectionId}", verify=False).text)
    except:
        return None


def check_user_is_deleted(userId):
    return bool(json.loads(
        requests.get(f"{ApiBase.api_address}/CheckUserIsDeleted/{userId}", verify=False).text))


def check_users_are_combinable(user1, user2):
    return bool(
        json.loads(
            requests.get(f"{ApiBase.api_address}/CheckUsersAreCombinableRT/{user1}/{user2}", verify=False).text))


def get_common_langs(user1Id, user2Id, localisationId):
    return requests.get(f"{ApiBase.api_address}/RetreiveCommonLanguages/{user1Id}/{user2Id}/{localisationId}",
                        verify=False).text


def check_user_has_requests(userId):
    try:
        return bool(
            json.loads(requests.get(f"{ApiBase.api_address}/CheckUserHasRequests/{userId}", verify=False).text))
    except:
        return None


def check_user_has_notifications(userId):
    try:
        return bool(
            json.loads(requests.get(f"{ApiBase.api_address}/CheckUserHasNotifications/{userId}", verify=False).text))
    except:
        return None


def check_user_has_premium(userId):
    try:
        return bool(
            json.loads(requests.get(f"{ApiBase.api_address}/CheckUserHasPremium/{userId}", verify=False).text))
    except:
        return None


def check_user_uses_personality(userId):
    try:
        return bool(
            json.loads(requests.get(f"{ApiBase.api_address}/CheckUserUsesPersonality/{userId}", verify=False).text))
    except:
        return None


def check_user_in_a_blacklist(userId, encounteredUser):
    try:
        return bool(
            json.loads(
                requests.get(f"{ApiBase.api_address}/CheckEncounteredUserIsInBlackList/{userId}/{encounteredUser}",
                             verify=False).text))
    except:
        return None


def update_user(model: user_models.UserNew) -> Union[Response, None]:
    try:
        d = model.to_json()
        return requests.post(f"{ApiBase.api_address}/update-user", d, headers={
            "Content-Type": "application/json"}, verify=False)
    except:
        return None


def register_user(model: user_models.UserNew) -> Union[Response, None]:
    try:
        d = model.to_json()
        return requests.post(f"{ApiBase.api_address}/user-register", d, headers={
            "Content-Type": "application/json"}, verify=False)
    except:
        return None


def get_user_language_limit(userId):
    try:
        return int(
            json.loads(
                requests.get(f"{ApiBase.api_address}/GetUserMaximumLanguageCount/{userId}", verify=False).text))
    except:
        return None


def get_user_limitations(userId):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/limitations/{userId}", verify=False).text)
    except:
        return None


def get_user_basic_info(userId):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/basic-info/{userId}", verify=False).text)
    except:
        return None


def get_user_app_language(userId):
    try:
        return requests.get(f"{ApiBase.api_address}/GetUserAppLanguage/{userId}", verify=False).text
    except:
        return None


def suggest_languages(language):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/suggest-languages?language={language}", verify=False).text)
    except:
        return None


def suggest_countries(country):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/suggest-countries?country={country}", verify=False).text)
    except:
        return None


def suggest_cities(city):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/suggest-cities?city={city}", verify=False).text)
    except:
        return None


def get_user_active_reply(userId):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/GetActiveAutoReply/{userId}", verify=False).text)
    except:
        return None


def get_user_request(requestId):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/CheckUserHasRequest/{requestId}", verify=False).text)
    except:
        return None


def get_user_info(userId: int) -> Union[user_models.UserInfo, None]:
    try:
        response = requests.get(f"{ApiBase.api_address}/user-info/{userId}", verify=False)
        data = response.json()

        return user_models.UserInfo(data)
    except:
        return None


def get_user_settings(userId: int) -> Union[user_models.UserSettings, None]:
    try:
        response = requests.get(f"{ApiBase.api_address}/user-settings/{userId}", verify=False)
        data = response.json()

        return user_models.UserSettings(data)
    except:
        return None


def get_sponsor_info(userId):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/GetSponsorInfo/{userId}", verify=False).text)
    except:
        return None


def get_sponsor_languages(userId):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/GetSponsorLanguages/{userId}", verify=False).text)
    except:
        return None


def get_user_base_info(userId):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/GetUserBaseInfo/{userId}", verify=False).text)
    except:
        return None


def get_user_requests(userId):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/user-requests/{userId}", verify=False).text)
    except:
        return None


def get_user_notifications(userId):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/user-notifications/{userId}", verify=False).text)
    except:
        return None


def delete_user_requests(userId):
    try:
        return json.loads(requests.delete(f"{ApiBase.api_address}/DeleteUserRequests/{userId}", verify=False).text)
    except:
        return None


def delete_user_request(requestId):
    try:
        return json.loads(requests.delete(f"{ApiBase.api_address}/DeleteUserRequest/{requestId}", verify=False).text)
    except:
        return None


def delete_user_notification(notificationId):
    try:
        return bool(
            json.loads(requests.get(f"{ApiBase.api_address}/delete-notification/{notificationId}",
                                    verify=False).text))
    except:
        return None


def start_program_in_debug_mode(userIds: list[str]):  # TODO: remove in production
    data = json.dumps(userIds)
    requests.post(f"{ApiBase.api_address}/debug", data,
                                        headers={"Content-Type": "application/json"},
                                        verify=False)


def get_request_sender(requestId):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/GetRequestSender/{requestId}", verify=False).text)
    except:
        return None


def get_user_list(userId):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/user-list", params={
            "userId": userId
        }, verify=False).text)
    except:
        return None


def get_free_user_list(userId):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/GetUserList/FreeSearch/{userId}", verify=False).text)
    except:
        return None


def get_user_list_by_tags(getUserByTagsModel):
    try:
        d = json.dumps(getUserByTagsModel)
        return json.loads(requests.post(f"{ApiBase.api_address}/GetUserByTags", d,
                                        headers={"Content-Type": "application/json"},
                                        verify=False).text)
    except:
        return None


def user_invitation_link(invitationId, userId):
    try:
        return bool(
            json.loads(requests.get(f"{ApiBase.api_address}/InviteUser/{invitationId}/{userId}", verify=False).text))
    except:
        return None


def register_user_request(senderId, receiverId, isLikedBack, description=""):
    try:
        data = {
            "senderId": senderId,
            "userId": receiverId,
            "isLikedBack": isLikedBack,
            "description": description
        }

        d = json.dumps(data)
        return requests.post(f"{ApiBase.api_address}/user-request", d,
                             headers={"Content-Type": "application/json"},
                             verify=False).text
    except:
        return None


def answer_user_request(requestId, reaction) -> str:
    return requests.get(f"{ApiBase.api_address}/answer-user-request", params={
        "requestId": requestId,
        "reaction": reaction,
    }, verify=False).text


def decline_user_request(userId, encounteredUser):
    return requests.get(f"{ApiBase.api_address}/decline-user-request", params={
        "userId": userId,
        "encounteredUser": encounteredUser,
    }, verify=False)


def register_user_encounter(current_user_id, user_id, section_id):
    try:
        data = {
            "userId": current_user_id,
            "encounteredUserId": user_id,
            "section": section_id,
        }

        d = json.dumps(data)

        de = requests.post(f"{ApiBase.api_address}/RegisterUserEncounter", d,
                           headers={"Content-Type": "application/json"},
                           verify=False).text
        return json.loads(de)
    except:
        return None


def register_user_encounter_familiator(current_user_id, user_id):
    try:
        register_user_encounter(current_user_id, user_id, 2)
    except:
        return None


def register_user_encounter_rt(current_user_id, user_id):
    try:
        register_user_encounter(current_user_id, user_id, 5)
    except:
        return None


def get_all_user_achievements(userId):
    return json.loads(requests.get(f"{ApiBase.api_address}/user-achievements/{userId}", verify=False).text)


def get_user_achievement(userId, achievementId):
    return json.loads(requests.get(f"{ApiBase.api_address}/user-achievement/{userId}/{achievementId}", verify=False).text)


def get_all_user_achievements_admin(userId):
    return json.loads(requests.get(f"{ApiBase.api_address}/GetUserAchievementsAsAdmin/{userId}", verify=False).text)


def get_active_user_balance(userId):
    try:
        return json.loads(
            requests.get(f"{ApiBase.api_address}/user-balance/{userId}", verify=False).text)
    except:
        return None


def top_up_user_balance(userId, amount, description):
    return json.loads(requests.get(f"{ApiBase.api_address}/TopUpUserWalletBalance/{userId}/{amount}/{description}",
                                   verify=False).text)


def check_user_balance_is_sufficient(userId, cost):
    return bool(json.loads(
        requests.get(f"{ApiBase.api_address}/CheckBalanceIsSufficient/{userId}/{cost}", verify=False).text))


def check_should_turn_off_personality(userId):
    return bool(json.loads(
        requests.get(f"{ApiBase.api_address}/CheckShouldTurnOffPersonality/{userId}", verify=False).text))


def check_promo_is_valid(userId, promo, isActivatedBeforeRegistration):
    return bool(json.loads(
        requests.get(f"{ApiBase.api_address}/CheckPromoIsCorrect/{userId}/{promo}/{isActivatedBeforeRegistration}",
                     verify=False).text))


def grant_premium_for_points(userId, cost, dayDuration):
    return requests.get(f"{ApiBase.api_address}/GrantPremiumToUser/{userId}/{float(cost)}/{dayDuration}/{points}", verify=False)


def grant_premium_for_real_money(userId, cost, dayDuration, currency):
    c = float(cost.replace(',', '.'))
    return requests.get(f"{ApiBase.api_address}/GrantPremiumToUser/{userId}/{c}/{dayDuration}/{currency}", verify=False)


def purchase_effect_for_points(userId, effectId, cost, count=1):
    return requests.get(f"{ApiBase.api_address}/PurchaseEffect/{userId}/{effectId}/{float(cost)}/{points}/{count}", verify=False)


def purchase_test(userId, testId, cost, currency, language):
    price = cost

    if type(cost) is str:
        price = cost.replace(',', '.')

    return requests.get(f"{ApiBase.api_address}/purchase-test", params={
        "userId": userId,
        "testId": testId,
        "cost": float(price),
        "currency": currency,
        "language": language
    }, verify=False)


def check_user_has_effect(userId, effectId):
    return bool(
        json.loads(requests.get(f"{ApiBase.api_address}/CheckUserHasEffect/{userId}/{effectId}", verify=False).text))


def purchase_effect_for_real_money(userId, effectId, cost, currency, count=1):
    return requests.get(f"{ApiBase.api_address}/PurchaseEffect/{userId}/{effectId}/{float(cost.replace(',', '.'))}/{currency}/{count}", verify=False)


def purchase_PP_for_points(userId, cost, count=1):
    return requests.get(f"{ApiBase.api_address}/PurchesPPForPoints/{userId}/{float(cost)}/{count}", verify=False)


def purchase_PP_for_real_money(userId, cost, currency, count=1):
    return requests.get(f"{ApiBase.api_address}/PurchesPPForRealMoney/{userId}/{float(cost.replace(',', '.'))}/{currency}/{count}", verify=False)


def purchase_points_for_real_money(userId, cost, currency, amount):
    return requests.get(f"{ApiBase.api_address}/PurchasePoints", verify=False, params={
        "userId": userId,
        "cost": float(cost.replace(',', '.')),
        "currency": currency,
        "amount": amount
    })

def switch_admin_status(userId):
    admin_switch_result = {
        0: "Sorry, you were not authorized as an admin",
        1: "Done. Your status had been changed"
    }

    return admin_switch_result[int(json.loads(requests.get(f"{ApiBase.api_address}/SwitchAdminStatus/{userId}",
                                                           verify=False).text))]


def switch_ocean_status(userId):
    return requests.get(f"{ApiBase.api_address}/switch-ocean-usage/{userId}",
                        verify=False)


def switch_familiarity_status(userId):
    return requests.get(f"{ApiBase.api_address}/SwitchIncreasedFamiliarity/{userId}",
                        verify=False)


def switch_hint_status(userId):
    return requests.get(f"{ApiBase.api_address}/set-hint-status/{userId}",
                        verify=False)


def switch_comment_status(userId):
    return requests.get(f"{ApiBase.api_address}/set-comment-status/{userId}",
                        verify=False)


def get_increased_familiarity_status(userId):
    return bool(json.loads(requests.get(f"{ApiBase.api_address}/GetUserIncreasedFamiliarity/{userId}",
                                        verify=False).text))


def update_user_status(userId, status):
    return bool(json.loads(
        requests.get(f"{ApiBase.api_address}/UpdateUserNickname/{userId}/{status}", verify=False).text))


def get_admin_status(userId):
    data = {
        False: "Disabled",
        True: "Enabled"
    }

    d = requests.get(f"{ApiBase.api_address}/GethAdminStatus/{userId}",
                     verify=False)
    if d.text:
        return data[bool(json.loads(d.text))]
    return None


def set_user_currency(userId, currency):
    response = requests.get(f"{ApiBase.api_address}/SetUserCurrency/{userId}/{currency}",
                            verify=False)

    if 100 < response.status_code < 300:
        return True

    return False


def register_adventure(adventureData):

        d = json.dumps(adventureData)
        return requests.post(f"{ApiBase.api_address}/RegisterAdventure", d,
                             headers={"Content-Type": "application/json"},
                             verify=False).text


def change_adventure(adventureData):
    try:
        d = json.dumps(adventureData)
        return requests.post(f"{ApiBase.api_address}/ChangeAdventure", d,
                             headers={"Content-Type": "application/json"},
                             verify=False).status_code
    except:
        return None


def set_adventure_group_link(request):
    try:
        d = json.dumps(request)
        return int(requests.post(f"{ApiBase.api_address}/adventure-group-id", d,
                                 headers={"Content-Type": "application/json"},
                                 verify=False).text)
    except:
        return None


def get_adventure_template(adventureId):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/adventure-template/{adventureId}", verify=False).text)
    except:
        return None


def get_adventures(userId):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/get-adventures?userId={userId}", verify=False).text)
    except:
        return None


def get_my_adventures(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/user-adventures?userId={userId}", verify=False).text)
    except:
        return None


def send_adventure_request(adventureId, userId):
    try:
        return int(requests.get(f"{ApiBase.api_address}/adventure-request?id={adventureId}&userId={userId}", verify=False).text)
    except:
        return None


def send_adventure_request_by_code(userId, code):
    try:
        data = {
            "userId": userId,
            "invitationCode": code
        }
        d = json.dumps(data)
        return int(requests.post(f"{ApiBase.api_address}/SendAdventureRequestByCode", d,
                                 headers={"Content-Type": "application/json"},
                                 verify=False).text)
    except:
        return None


def process_participation_request(adventureId, userId, status):
    try:
        # TODO: perhaps change return type to enum
        return bool(requests.get(f"{ApiBase.api_address}/process-adventure-request/{adventureId}/{userId}/{status}",
                                 verify=False).text)
    except:
        return None


def save_template(templateData):
    try:
        d = json.dumps(templateData)
        return requests.post(f"{ApiBase.api_address}/SaveTemplate", d,
                             headers={"Content-Type": "application/json"},
                             verify=False).status_code
    except:
        return None


def get_template(templateId):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/adventure-template/{templateId}", verify=False).text)
    except:
        return None


def get_templates(userId):
    try:
        return json.loads(requests.get(f"{ApiBase.api_address}/adventure-templates/{userId}", verify=False).text)
    except:
        return None


def delete_template(templateId):
    try:
        return int(requests.delete(f"{ApiBase.api_address}/delete-template/{templateId}", verify=False).text)
    except:
        return None


def delete_attendee(adventureId, attendeeId):
    try:
        return int(
            requests.delete(f"{ApiBase.api_address}/delete-attendee/{adventureId}/{attendeeId}", verify=False).text)
    except:
        return None


def delete_adventure(adventureId, userId):
    try:
        return requests.delete(f"{ApiBase.api_address}/adventure?id={adventureId}&userId={userId}", verify=False).text
    except:
        return None


def get_report_reasons(language):
    try:
        return json.loads(
            requests.get(f"{ApiBase.api_address}/report-reasons", headers={"Accept-Language": language},
                         verify=False).text)
    except:
        return None


def get_app_languages():
    try:
        return json.loads(
            requests.get(f"{ApiBase.api_address}/app-languages", verify=False).text)
    except:
        return None


def get_payment_currencies():
    try:
        return json.loads(
            requests.get(f"{ApiBase.api_address}/payment-currencies", verify=False).text)
    except:
        return None


def report_user(report_data):
    d = json.dumps(report_data)

    requests.post(f"https://localhost:44381/report-user", d, headers={
        "Content-Type": "application/json"}, verify=False)


def report_adventure(report_data):
    d = json.dumps(report_data)

    requests.post(f"https://localhost:44381/report-adventure", d, headers={
        "Content-Type": "application/json"}, verify=False)


def delete_user_profile(userId, message):
    try:
        d = json.dumps({
            "userId": userId,
            "message": message
        })

        return requests.post(f"{ApiBase.api_address}/delete-user", d,
                             headers={"Content-Type": "application/json"},
                             verify=False).status_code
    except:
        return None


def restore_user_profile(userId):
    try:
        return json.loads(
            requests.get(f"{ApiBase.api_address}/restore-user",
                         verify=False, params={"userId": userId}).text)
    except:
        return None

def send_feedback(payload: dict):
    try:
        d = json.dumps(payload)
        requests.post("https://localhost:44381/feedback", d, headers={
            "Content-Type": "application/json"}, verify=False)

    except:
        return None


def get_active_effect(user_id):
    try:
        return json.loads(requests.get(f"https://localhost:44381/active-effects/{user_id}", verify=False).text)
    except:
        return None


def set_user_story(userId, story):
    try:
        d = json.dumps({
            "userId": userId,
            "story": story
        })

        return requests.post(f"{ApiBase.api_address}/user-story", d,
                             headers={"Content-Type": "application/json"},
                             verify=False).status_code
    except:
        return None

def remove_user_story(userId):
    try:
        return requests.delete(f"{ApiBase.api_address}/user-story", verify=False, params={"userId": userId})
    except Exception as ex:
        return None

def add_advertisement(payload: advertisement_models.AdvertisementNew) -> Union[Response, None]:
    try:
        data = payload.to_json()

        response = requests.post("https://localhost:44381/advertisement", data, headers={
            "Content-Type": "application/json"}, verify=False)
        return response

    except:
        return None


def update_advertisement(payload: advertisement_models.AdvertisementUpdate) -> Union[Response, None]:
    try:
        data = payload.to_json()

        response = requests.put("https://localhost:44381/advertisement", data, headers={
            "Content-Type": "application/json"}, verify=False)
        return response

    except:
        return None


def get_advertisement_list(userId) -> Union[list[advertisement_models.AdvertisementItem], None]:
    try:
        response = requests.get(f"{ApiBase.api_address}/advertisement-list/{userId}", verify=False)
        advertisements = response.json()

        return [advertisement_models.AdvertisementItem(advertisement) for advertisement in advertisements]
    except:
        return


def get_advertisement_info(adId) -> Union[advertisement_models.Advertisement, None]:

    response = requests.get(f"{ApiBase.api_address}/advertisement/{adId}", verify=False)
    advertisement = response.json()

    return advertisement_models.Advertisement(advertisement)


def delete_advertisement(advertisementId) -> Union[Response, None]:
    try:
        response = requests.delete(f"{ApiBase.api_address}/advertisement/{advertisementId}", verify=False)

        return response
    except:
        return


def get_all_advertisement_priorities() -> Union[list[generic_models.LocalizedEnum], None]:
    try:
        response = requests.get(f"{ApiBase.api_address}/priorities", verify=False)
        priorities = response.json()

        return [generic_models.LocalizedEnum(priority) for priority in priorities]
    except:
        return


def get_advertisement_economy_monthly_statistics(user_id, advertisement_id) \
        -> list[advertisement_models.StatisticsEconomy] | None:
    try:
        payload = create_statistics_request_model()
        params = {"advertisementId": advertisement_id}

        response = ApiBase.create_post_request_with_api_model(payload, f"statistics/{user_id}",
                                                              params)
        stats = response.json()

        return advertisement_models.Statistics.unpack(stats)

    except:
        return None


def get_advertisement_engagement_monthly_statistics(user_id, advertisement_id) \
        -> list[advertisement_models.StatisticsEconomy] | None:
    try:
        payload = create_statistics_request_model()
        params = {"advertisementId": advertisement_id}

        response = ApiBase.create_post_request_with_api_model(payload, f"statistics/{user_id}",
                                                              params)
        stats = response.json()

        return advertisement_models.Statistics.unpack(stats)

    except:
        return None


def create_statistics_request_model():
    model = advertisement_models.StatisticsGet()

    now = datetime.datetime.now()
    max_days = calendar.monthrange(now.year, now.month)[1]

    model.from_ = datetime.datetime(now.year, now.month, 1)
    model.to = datetime.datetime(now.year, now.month, max_days)

    return model.to_json()
