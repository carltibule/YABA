import { callExternalApi } from "../apiHelper";

const apiServerUrl = `${process.env.REACT_APP_API_BASE_URL}/v1/Tags`;

export const getAllTags = async(accessToken) => {
    const config = {
        url: `${apiServerUrl}`,
        method: "GET",
        headers: {
            "content-type": "application/json",
            Authorization: `Bearer ${accessToken}`
        },
    };

    return await callExternalApi({config});
}