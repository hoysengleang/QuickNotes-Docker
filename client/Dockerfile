FROM node:20 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
ENV VITE_API_BASE_URL=http://localhost:5000/api
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]