# https://www.knowledgehut.com/blog/web-development/how-to-dockerize-react-app 
FROM node:17-alpine as builder
WORKDIR /app
COPY package-lock.json  .
RUN npm install
COPY . .
RUN npm build

FROM nginx:latest
WORKDIR /usr/share/nginx/html
RUN rm -rf ./*
COPY --from=builder /app/build .
ENTRYPOINT ["nginx", "-g", "daemon off;"]