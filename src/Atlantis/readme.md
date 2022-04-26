# Atlantis Framework 
Framework Backend para construccion de aplicaciones basadas en DDD para multiples consumidores. Atlantis permite construir aplicaciones de servidor donde se concentra toda la logica de negocio. Permite utilizar separacion de
responsabilidades al aislar completamente la parte de lecturas y escrituras. Define los mecanismos internos para agregar reactividad al sistema, permitiendo la publicacion y el consumo de eventos de dominio y las reacciones de
los eventos hacia otros nuevos comandos en colas que se procesan alternadamente

## Componentes principales 

## Cqrs
Atlantis implementa los mecanismos para ejecutar logica de negocio, o casos de uso a traves de los comandos. Los comandos nos aseguran esa integridad y transaccionalidad para asegurar que los
cambios realizados en una entidad sean guardados solamente si todo salio bien. Por otro lado, Atlantis especifica queries que estan optimizada la ejecucion para las lecturas rapidas con modelos a la medida sin dependencia de ningun ORM principal. Atlantis viene integrado con MediatR para la ejecucion de comandos y queries. 

## Domain Driven Design
Atlantis cuenta con una seria de librerias que permiten hacer uso de DDD centralizando completamente la logica de negocio en entidades de tipo DDD. Tiene clases bases para definir enumeraciones, entidades, objetos de valor, raices agregadas y eventos de dominio. Ademas, cuenta con los mecanismos para recolectar estos eventos de dominio y despacharlos a traves de cqrs. Ademas define el acceso a estas entidades a traves de los repositorios que pueden ser implementados por los clientes de forma libre con o sin orms, a como mas convenga.