import {smtpServerTab as ActionTypes}  from "../constants/actionTypes";
import smtpServerService from "../services/smtpServerService";
import localization from "../localization";
import Promise from "es6-promise";

const smtpServeTabActions = {
    loadSmtpServerInfo(callback) {       
        return (dispatch) => {
            dispatch({
                type: ActionTypes.LOAD_SMTP_SERVER_TAB               
            });        
            
            smtpServerService.getSmtpSettings().then(response => {
                Promise.all([dispatch({
                    type: ActionTypes.LOADED_SMTP_SERVER_TAB,
                    payload: {
                        smtpServerInfo: response
                    }
                })]).then(() => {
                    if (typeof callback === "function") {
                        callback();
                    }
                }); 
            }).catch(() => {
                dispatch({
                    type: ActionTypes.ERROR_LOADING_SMTP_SERVER_TAB,
                    payload: {
                        errorMessage: localization.get("errorMessageLoadingSmtpServerTab")
                    }
                });
            });        
        };
    },
    changeSmtpServerMode(smtpServeMode, callback) {
        return (dispatch) => {
            Promise.all([dispatch({
                type: ActionTypes.CHANGE_SMTP_SERVER_MODE,
                payload: {
                    smtpServeMode
                }
            })]).then(() => {
                if (typeof callback === "function") {
                    callback();
                }
            });
        };
    },
    changeSmtpAuthentication(smtpAuthentication) {
        return (dispatch) => {
            dispatch({
                type: ActionTypes.CHANGE_SMTP_AUTHENTICATION,
                payload: {
                    smtpAuthentication
                }
            });
        };
    },
    changeSmtpConfigurationValue(key, value, passCheck) {
        return (dispatch) => {
            dispatch({
                type: ActionTypes.CHANGE_SMTP_CONFIGURATION_VALUE,
                payload: { 
                    field: key,
                    value,
                    passCheck
                }
            });  
        };
    },
    updateSmtpServerSettings(parameters, callback) {       
        return (dispatch) => {
            dispatch({
                type: ActionTypes.UPDATE_SMTP_SERVER_SETTINGS               
            });        
            
            smtpServerService.updateSmtpSettings(parameters).then(response => {
                let payload = {
                    success: response.success,
                    errors: response.errors,
                    providerChanged: response.providerChanged || false
                };
                Promise.all([dispatch({
                    type: ActionTypes.UPDATED_SMTP_SERVER_SETTINGS,
                    payload: payload
                })]).then(() => {
                    if (typeof callback === "function") {
                        callback(payload);
                    }
                });
            }).catch(() => {
                dispatch({
                    type: ActionTypes.ERROR_UPDATING_SMTP_SERVER_SETTINGS,
                    payload: {
                        errorMessage: localization.get("errorMessageUpdatingSmtpServerTab")
                    }
                });
            });        
        };
    },
    sendTestEmail(parameters) {       
        return (dispatch) => {
            dispatch({
                type: ActionTypes.SEND_TEST_EMAIL               
            });        
            
            smtpServerService.sendTestEmail(parameters).then(response => {
                dispatch({
                    type: ActionTypes.SENT_TEST_EMAIL,
                    payload: {
                        success: response.success,
                        infoMessage: response.confirmationMessage,
                        errorMessage: response.errMessage
                    }
                });  
            }).catch((data) => {
                let response = JSON.parse(data.responseText);
                dispatch({
                    type: ActionTypes.ERROR_SENDING_TEST_EMAIL,
                    payload: {
                        errorMessage: response.errMessage
                    }
                });
            });        
        };
    },
    loadAuthProviders(callback) {       
        return (dispatch) => {
            dispatch({
                type: ActionTypes.LOAD_OAUTH_PROVIDERS               
            });        
            
            smtpServerService.getOAuthProviders().then(response => {
                Promise.all([dispatch({
                    type: ActionTypes.LOADED_OAUTH_PROVIDERS,
                    payload: {
                        authProviders: response,
                        providerChanged: false
                    }
                })]).then(() => {
                    if (typeof callback === "function") {
                        callback();
                    }
                }); 
            }).catch(() => {
                dispatch({
                    type: ActionTypes.ERROR_LOADING_SMTP_SERVER_TAB,
                    payload: {
                        errorMessage: localization.get("errorMessageLoadingSmtpServerTab")
                    }
                });
            });        
        };
    }
};

export default smtpServeTabActions;