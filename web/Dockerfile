FROM node:18-alpine3.15 AS build-env
WORKDIR /App

COPY . ./
RUN npm install
RUN npm run build

FROM nginx:1.23.2-alpine
WORKDIR /App
COPY --from=build-env /App/dist /usr/share/nginx/html