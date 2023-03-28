import { callExternalApi } from "../apiHelper";

const apiServerUrl = `${process.env.REACT_APP_API_BASE_URL}/v1/Misc`;

export const getWebsiteMetaData = async(accessToken, url) => {
    const config = {
        url: `${apiServerUrl}/GetWebsiteMetaData`,
        method: "GET",
        headers: {
            "content-type": "application/json",
            Authorization: `Bearer ${accessToken}`
        },
        params: {
            'url': url
        }
    };

    return await callExternalApi({config});
}