import json
import requests

# Used to for translating Accept-Language header
languages = {
    0: "en-US",
    1: "ru-RU",
    2: "uk-UA"
}

api_address = ""


def check_user_exists(userId):
    return bool(json.loads(requests.get(f"{api_address}/CheckUserExists/{userId}", verify=False).text))


def check_user_is_sponsor(userId):
    return bool(json.loads(
        requests.get(f"{api_address}/CheckUserIsSponsor/{userId}", verify=False).text))


def check_user_is_awaiting(userId):
    return bool(json.loads(
        requests.get(f"{api_address}/CheckUserIsAwaiting/{userId}", verify=False).text))


def check_user_is_awaiting_by_username(username):
    return bool(json.loads(
        requests.get(f"{api_address}/CheckUserIsAwaitingByUsername/{username}", verify=False).text))


def check_sponsor_is_maxed(userId):
    return bool(json.loads(
        requests.get(f"{api_address}/CheckSponsorIsMaxed/{userId}", verify=False).text))


def check_user_keyword_is_correct(userId, keyword):
    return bool(json.loads(
        requests.get(f"{api_address}/CheckUserKeyWordIsCorrect/{userId}/{keyword}", verify=False).text))


def check_user_is_admin(userId):
    return bool(
        json.loads(requests.get(f"{api_address}/CheckUserIsAdmin/{userId}", verify=False).text))


def check_user_is_postponed(userId):
    return bool(json.loads(
        requests.get(f"{api_address}/CheckSponsorIsPostponed/{userId}", verify=False).text))


def check_user_is_registered(userId):
    return bool(json.loads(
        requests.get(f"{api_address}/CheckUserIsRegistered/{userId}", verify=False).text))


def check_user_has_visited_section(userId, sectionId):
    try:
        return bool(json.loads(
            requests.get(f"{api_address}/CheckUserHasVisitedSection/{userId}/{sectionId}",
                         verify=False).text))
    except:
        return None


def check_user_is_banned(userId):
    return bool(json.loads(
        requests.get(f"{api_address}/CheckUserIsBanned/{userId}", verify=False).text))


def check_user_is_busy(userId):
    try:
        return bool(json.loads(
            requests.get(f"{api_address}/CheckUserIsBusy/{userId}", verify=False).text))
    except:
        return None


def switch_user_busy_status(userId, sectionId):
    try:
        return json.loads(
            requests.get(f"{api_address}/SwhitchUserBusyStatus/{userId}/{sectionId}", verify=False).text)
    except:
        return None


def check_user_is_deleted(userId):
    return bool(json.loads(
        requests.get(f"{api_address}/CheckUserIsDeleted/{userId}", verify=False).text))


def check_users_are_combinable(user1, user2):
    return bool(
        json.loads(
            requests.get(f"{api_address}/CheckUsersAreCombinableRT/{user1}/{user2}", verify=False).text))


def get_common_langs(user1Id, user2Id, localisationId):
    return requests.get(f"{api_address}/RetreiveCommonLanguages/{user1Id}/{user2Id}/{localisationId}",
                        verify=False).text


def check_user_has_requests(userId):
    try:
        return bool(
            json.loads(requests.get(f"{api_address}/CheckUserHasRequests/{userId}", verify=False).text))
    except:
        return None


def check_user_has_notifications(userId):
    try:
        return bool(
            json.loads(requests.get(f"{api_address}/CheckUserHasNotifications/{userId}", verify=False).text))
    except:
        return None


def check_user_has_premium(userId):
    try:
        return bool(
            json.loads(requests.get(f"{api_address}/CheckUserHasPremium/{userId}", verify=False).text))
    except:
        return None


def check_user_uses_personality(userId):
    try:
        return bool(
            json.loads(requests.get(f"{api_address}/CheckUserUsesPersonality/{userId}", verify=False).text))
    except:
        return None


def check_user_in_a_blacklist(userId, encounteredUser):
    try:
        return bool(
            json.loads(
                requests.get(f"{api_address}/CheckEncounteredUserIsInBlackList/{userId}/{encounteredUser}",
                             verify=False).text))
    except:
        return None


def get_user_language_limit(userId):
    try:
        return int(
            json.loads(
                requests.get(f"{api_address}/GetUserMaximumLanguageCount/{userId}", verify=False).text))
    except:
        return None


def get_user_limitations(userId):
    try:
        return json.loads(requests.get(f"{api_address}/limitations/{userId}", verify=False).text)
    except:
        return None


def get_user_basic_info(userId):
    try:
        return json.loads(requests.get(f"{api_address}/basic-info/{userId}", verify=False).text)
    except:
        return None


def get_user_app_language(userId):
    try:
        return int(
            json.loads(requests.get(f"{api_address}/GetUserAppLanguage/{userId}", verify=False).text))
    except:
        return None


def get_user_active_reply(userId):
    try:
        return json.loads(requests.get(f"{api_address}/GetActiveAutoReply/{userId}", verify=False).text)
    except:
        return None


def get_user_request(requestId):
    try:
        return json.loads(requests.get(f"{api_address}/CheckUserHasRequest/{requestId}", verify=False).text)
    except:
        return None


def get_user_info(userId):
    try:
        return json.loads(requests.get(f"{api_address}/UserInfo/{userId}", verify=False).text)
    except:
        return None


def get_sponsor_info(userId):
    try:
        return json.loads(requests.get(f"{api_address}/GetSponsorInfo/{userId}", verify=False).text)
    except:
        return None


def get_sponsor_languages(userId):
    try:
        return json.loads(requests.get(f"{api_address}/GetSponsorLanguages/{userId}", verify=False).text)
    except:
        return None


def get_user_base_info(userId):
    try:
        return json.loads(requests.get(f"{api_address}/GetUserBaseInfo/{userId}", verify=False).text)
    except:
        return None


def get_user_requests(userId):
    try:
        return json.loads(requests.get(f"{api_address}/GetUserRequests/{userId}", verify=False).text)
    except:
        return None


def get_user_notifications(userId):
    try:
        return json.loads(requests.get(f"{api_address}/GetUserNotifications/{userId}", verify=False).text)
    except:
        return None


def delete_user_requests(userId):
    try:
        return json.loads(requests.delete(f"{api_address}/DeleteUserRequests/{userId}", verify=False).text)
    except:
        return None


def delete_user_request(requestId):
    try:
        return json.loads(requests.delete(f"{api_address}/DeleteUserRequest/{requestId}", verify=False).text)
    except:
        return None


def delete_user_notification(notificationId):
    try:
        return bool(
            json.loads(requests.get(f"{api_address}/SendNotificationConfirmationCode/{notificationId}",
                                    verify=False).text))
    except:
        return None


def start_program_in_debug_mode(bot):  # TODO: remove in production
    requests.get(f"{api_address}/SetDebugProperties", verify=False)


def get_request_sender(requestId):
    try:
        return json.loads(requests.get(f"{api_address}/GetRequestSender/{requestId}", verify=False).text)
    except:
        return None


def get_user_list(userId):
    try:
        return json.loads(requests.get(f"{api_address}/GetUserList/{userId}", verify=False).text)
    except:
        return None


def get_user_list_turnOffPersonality(userId):
    try:
        return json.loads(requests.get(f"{api_address}//GetUserList/TurnOffP/{userId}", verify=False).text)
    except:
        return None


def get_free_user_list(userId):
    try:
        return json.loads(requests.get(f"{api_address}//GetUserList/FreeSearch/{userId}", verify=False).text)
    except:
        return None


def get_user_list_by_tags(getUserByTagsModel):
    try:
        d = json.dumps(getUserByTagsModel)
        return json.loads(requests.post(f"{api_address}/GetUserByTags", d,
                                        headers={"Content-Type": "application/json"},
                                        verify=False).text)
    except:
        return None


def user_invitation_link(invitationId, userId):
    try:
        return bool(
            json.loads(requests.get(f"{api_address}/InviteUser/{invitationId}/{userId}", verify=False).text))
    except:
        return None


def register_user_request(senderId, receiverId, isLikedBack, description=""):
    try:
        data = {
            "userId": senderId,
            "userId1": receiverId,
            "isLikedBack": isLikedBack,
            "description": description
        }

        d = json.dumps(data)
        return requests.post(f"{api_address}/RegisterUserRequest", d,
                             headers={"Content-Type": "application/json"},
                             verify=False).text
    except:
        return None


def decline_user_request(user1, user2):
    try:
        return requests.get(f"{api_address}/DeclineRequest/{user1}/{user2}",
                            verify=False).text
    except:
        return None


def register_user_encounter(current_user_id, user_id, section_id):
    try:
        data = {
            "id": 0,
            "userId": current_user_id,
            "userId1": user_id,
            "sectionId": section_id,
        }

        d = json.dumps(data)

        de = requests.post(f"{api_address}/RegisterUserEncounter", d,
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
    return json.loads(requests.get(f"{api_address}/GetUserAchievements/{userId}", verify=False).text)


def get_all_user_achievements_admin(userId):
    return json.loads(requests.get(f"{api_address}/GetUserAchievementsAsAdmin/{userId}", verify=False).text)


def get_active_user_balance(userId):
    try:
        return json.loads(
            requests.get(f"{api_address}/GetActiveUserWalletBalance/{userId}", verify=False).text)
    except:
        return None


def top_up_user_balance(userId, amount, description):
    return json.loads(requests.get(f"{api_address}/TopUpUserWalletBalance/{userId}/{amount}/{description}",
                                   verify=False).text)


def check_user_balance_is_sufficient(userId, cost):
    return bool(json.loads(
        requests.get(f"{api_address}/CheckBalanceIsSufficient/{userId}/{cost}", verify=False).text))


def check_user_have_chosen_free_search(userId):
    return bool(json.loads(
        requests.get(f"{api_address}/CheckUserHaveChosenFreeParam/{userId}", verify=False).text))


def check_should_turnOf_personality(userId):
    return bool(json.loads(
        requests.get(f"{api_address}/CheckShouldTurnOffPersonality/{userId}", verify=False).text))


def check_promo_is_valid(userId, promo, isActivatedBeforeRegistration):
    return bool(json.loads(
        requests.get(f"{api_address}/CheckPromoIsCorrect/{userId}/{promo}/{isActivatedBeforeRegistration}",
                     verify=False).text))


def grant_premium_for_points(userId, cost, dayDuration):
    return json.loads(requests.get(f"{api_address}/GrantPremiumToUser/{userId}/{cost}/{dayDuration}/{1}",
                                   verify=False).text)


def grant_premium_for_real_money(userId, cost, dayDuration):
    return json.loads(requests.get(f"{api_address}/GrantPremiumToUser/{userId}/{cost}/{dayDuration}/{4}",
                                   verify=False).text)


def purchase_effect_for_points(userId, effectId, cost, count=1):
    return json.loads(
        requests.get(f"{api_address}/PurchaseEffect/{userId}/{effectId}/{cost}/1/{count}", verify=False).text)


def check_user_has_effect(userId, effectId):
    return bool(
        json.loads(requests.get(f"{api_address}/CheckUserHasEffect/{userId}/{effectId}", verify=False).text))


# TODO: Change called API endpoint
def purchase_effect_for_real_money(userId, effectId, cost):
    return json.loads(
        requests.get(f"{api_address}/GrantPremiumToUser/{userId}/{effectId}/{cost}/{4}", verify=False).text)


def purchase_PP_for_points(userId, cost, count=1):
    return json.loads(
        requests.get(f"{api_address}/PurchesPPForPoints/{userId}/{cost}/{count}", verify=False).text)


def switch_admin_status(userId):
    admin_switch_result = {
        0: "Sorry, you were not authorized as an admin",
        1: "Done. Your status had been changed"
    }

    return admin_switch_result[int(json.loads(requests.get(f"{api_address}/SwitchAdminStatus/{userId}",
                                                           verify=False).text))]


def switch_personality_status(userId):
    return requests.get(f"{api_address}/SwitchPersonalityUsage/{userId}",
                        verify=False)


def switch_familiarity_status(userId):
    return requests.get(f"{api_address}/SwitchIncreasedFamiliarity/{userId}",
                        verify=False)


def switch_hint_status(userId):
    return requests.get(f"{api_address}/set-hint-status/{userId}",
                        verify=False)


def switch_comment_status(userId):
    return requests.get(f"{api_address}/set-comment-status/{userId}",
                        verify=False)


def get_increased_familiarity_status(userId):
    return bool(json.loads(requests.get(f"{api_address}/GetUserIncreasedFamiliarity/{userId}",
                                        verify=False).text))


def update_user_status(userId, status):
    return bool(json.loads(
        requests.get(f"{api_address}/UpdateUserNickname/{userId}/{status}", verify=False).text))


def get_admin_status(userId):
    data = {
        False: "Disabled",
        True: "Enabled"
    }

    d = requests.get(f"{api_address}/GethAdminStatus/{userId}",
                     verify=False)
    if d.text:
        return data[bool(json.loads(d.text))]
    return None


def set_user_currency(userId, currency):
    response = requests.get(f"{api_address}/SetUserCurrency/{userId}/{currency}",
                            verify=False)

    if 100 < response.status_code < 300:
        return True

    return False


def register_adventure(adventureData):

        d = json.dumps(adventureData)
        return requests.post(f"{api_address}/RegisterAdventure", d,
                             headers={"Content-Type": "application/json"},
                             verify=False).text



def change_adventure(adventureData):
    try:
        d = json.dumps(adventureData)
        return requests.post(f"{api_address}/ChangeAdventure", d,
                             headers={"Content-Type": "application/json"},
                             verify=False).status_code
    except:
        return None


def set_adventure_group_link(request):
    try:
        d = json.dumps(request)
        return int(requests.post(f"{api_address}/adventure-group-id", d,
                                 headers={"Content-Type": "application/json"},
                                 verify=False).text)
    except:
        return None


def get_adventure(adventureId):
    try:
        return json.loads(requests.get(f"{api_address}/adventure-template/{adventureId}", verify=False).text)
    except:
        return None


def send_adventure_request_by_code(userId, code):
    try:
        data = {
            "userId": userId,
            "invitationCode": code
        }
        d = json.dumps(data)
        return int(requests.post(f"{api_address}/SendAdventureRequestByCode", d,
                                 headers={"Content-Type": "application/json"},
                                 verify=False).text)
    except:
        return None


def process_participation_request(adventureId, userId, status):
    try:
        # TODO: perhaps change return type to enum
        return bool(requests.get(f"{api_address}/process-adventure-request/{adventureId}/{userId}/{status}",
                                 verify=False).text)
    except:
        return None


def save_template(templateData):
    try:
        d = json.dumps(templateData)
        return requests.post(f"{api_address}/SaveTemplate", d,
                             headers={"Content-Type": "application/json"},
                             verify=False).status_code
    except:
        return None


def get_template(templateId):
    try:
        return json.loads(requests.get(f"{api_address}/adventure-template/{templateId}", verify=False).text)
    except:
        return None


def get_templates(userId):
    try:
        return json.loads(requests.get(f"{api_address}/adventure-templates/{userId}", verify=False).text)
    except:
        return None


def delete_template(templateId):
    try:
        return int(requests.delete(f"{api_address}/delete-template/{templateId}", verify=False).text)
    except:
        return None


def delete_attendee(adventureId, attendeeId):
    try:
        return int(
            requests.delete(f"{api_address}/delete-attendee/{adventureId}/{attendeeId}", verify=False).text)
    except:
        return None


def get_report_reasons(language):
    try:
        langHeader = languages[language]
        return json.loads(
            requests.get(f"{api_address}/report-reasons", headers={"Accept-Language": langHeader},
                         verify=False).text)
    except:
        return None
