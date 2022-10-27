import json
import requests


def check_user_exists(userId):
    return bool(json.loads(requests.get(f"https://localhost:44381/CheckUserExists/{userId}", verify=False).text))


def check_user_is_sponsor(userId):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckUserIsSponsor/{userId}", verify=False).text))


def check_user_is_awaiting(userId):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckUserIsAwaiting/{userId}", verify=False).text))


def check_user_is_awaiting_by_username(username):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckUserIsAwaitingByUsername/{username}", verify=False).text))


def check_sponsor_is_maxed(userId):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckSponsorIsMaxed/{userId}", verify=False).text))


def check_user_keyword_is_correct(userId, keyword):
    return bool(json.loads(requests.get(f"https://localhost:44381/CheckUserKeyWordIsCorrect/{userId}/{keyword}", verify=False).text))


def check_user_is_admin(userId):
    return bool(
        json.loads(requests.get(f"https://localhost:44381/CheckUserIsAdmin/{userId}", verify=False).text))


def check_user_is_postponed(userId):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckSponsorIsPostponed/{userId}", verify=False).text))


def check_user_is_registered(userId):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckUserIsRegistered/{userId}", verify=False).text))


def check_user_has_visited_section(userId, sectionId):
    try:
        return bool(json.loads(
            requests.get(f"https://localhost:44381/CheckUserHasVisitedSection/{userId}/{sectionId}", verify=False).text))
    except:
        return None


def check_user_is_banned(userId):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckUserIsBanned/{userId}", verify=False).text))


def check_user_is_busy(userId):
    try:
        return bool(json.loads(
            requests.get(f"https://localhost:44381/CheckUserIsBusy/{userId}", verify=False).text))
    except:
        return None


def switch_user_busy_status(userId):
    try:
        return bool(json.loads(
            requests.get(f"https://localhost:44381/SwhitchUserBusyStatus/{userId}", verify=False).text))
    except:
        return None


def check_user_is_deleted(userId):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckUserIsDeleted/{userId}", verify=False).text))


def check_users_are_combinable(user1, user2):
    return bool(
        json.loads(requests.get(f"https://localhost:44381/CheckUsersAreCombinableRT/{user1}/{user2}", verify=False).text))


def check_user_has_requests(userId):
    try:
        return bool(
            json.loads(requests.get(f"https://localhost:44381/CheckUserHasRequests/{userId}", verify=False).text))
    except:
        return None


def check_user_has_notifications(userId):
    try:
        return bool(
            json.loads(requests.get(f"https://localhost:44381/CheckUserHasNotifications/{userId}", verify=False).text))
    except:
        return None


def check_user_has_premium(userId):
    try:
        return bool(
            json.loads(requests.get(f"https://localhost:44381/CheckUserHasPremium/{userId}", verify=False).text))
    except:
        return None


def get_user_language_limit(userId):
    try:
        return int(
            json.loads(requests.get(f"https://localhost:44381/GetUserMaximumLanguageCount/{userId}", verify=False).text))
    except:
        return None


def get_user_request(requestId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/CheckUserHasRequest/{requestId}", verify=False).text)
    except:
        return None


def get_user_info(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/UserInfo/{userId}", verify=False).text)
    except:
        return None


def get_sponsor_info(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/GetSponsorInfo/{userId}", verify=False).text)
    except:
        return None


def get_sponsor_languages(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/GetSponsorLanguages/{userId}", verify=False).text)
    except:
        return None


def get_user_base_info(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/GetUserBaseInfo/{userId}", verify=False).text)
    except:
        return None


def get_user_requests(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/GetUserRequests/{userId}", verify=False).text)
    except:
        return None


def get_user_notifications(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/GetUserNotifications/{userId}", verify=False).text)
    except:
        return None


def delete_user_requests(userId):
    try:
        return json.loads(requests.delete(f"https://localhost:44381/DeleteUserRequests/{userId}", verify=False).text)
    except:
        return None


def delete_user_request(requestId):
    try:
        return json.loads(requests.delete(f"https://localhost:44381/DeleteUserRequest/{requestId}", verify=False).text)
    except:
        return None


def delete_user_notifications(notificationId):
    try:
        return bool(
            json.loads(requests.delete(f"https://localhost:44381/DeleteUserNotification/{notificationId}", verify=False).text))
    except:
        return None


def start_program_in_debug_mode(bot): # TODO: remove in production
    requests.get("https://localhost:44381/SetDebugProperties", verify=False)
    return json.loads(requests.get("https://localhost:44381/GetAllUsersIds", verify=False).text)


def get_user_list(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/GetUserList/{userId}", verify=False).text)
    except:
        return None


def user_invitation_link(invitationId, userId):
    try:
        return bool(
            json.loads(requests.get(f"https://localhost:44381/InviteUser/{invitationId}/{userId}", verify=False).text))
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
        return json.loads(requests.post(f"https://localhost:44381/RegisterUserRequest", d,
                                       headers={"Content-Type": "application/json"},
                                       verify=False).text)
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

        de = requests.post("https://localhost:44381/RegisterUserEncounter", d,
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
    return json.loads(requests.get(f"https://localhost:44381/GetUserAchievements/{userId}", verify=False).text)


def get_all_user_achievements_admin(userId):
    return json.loads(requests.get(f"https://localhost:44381/GetUserAchievementsAsAdmin/{userId}", verify=False).text)


def get_active_user_balance(userId):
    try:
        return json.loads(requests.get(f"https://localhost:44381/GetActiveUserWalletBalance/{userId}", verify=False).text)
    except:
        return None


def top_up_user_balance(userId, amount, description):
    return json.loads(requests.get(f"https://localhost:44381/TopUpUserWalletBalance/{userId}/{amount}/{description}", verify=False).text)


def check_user_balance_is_sufficient(userId, cost):
    return bool(json.loads(
        requests.get(f"https://localhost:44381/CheckBalanceIsSufficient/{userId}/{cost}", verify=False).text))


def grant_premium(userId, cost, dayDuration):
    return json.loads(requests.get(f"https://localhost:44381/GrantPremiumToUser/{userId}/{cost}/{dayDuration}", verify=False).text)


def switch_admin_status(userId):
    admin_switch_result = {
        0: "Sorry, you were not authorized as an admin",
        1: "Done. Your status had been changed"
    }

    return admin_switch_result[int(json.loads(requests.get(f"https://localhost:44381/SwitchAdminStatus/{userId}",
                                                           verify=False).text))]


def get_admin_status(userId):
    data = {
        False: "Disabled",
        True: "Enabled"
    }

    d = requests.get(f"https://localhost:44381/GethAdminStatus/{userId}",
                                             verify=False)
    if d.text:
        return data[bool(json.loads(d.text))]
    return None