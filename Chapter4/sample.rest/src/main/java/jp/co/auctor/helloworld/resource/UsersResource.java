package jp.co.auctor.helloworld.resource;

import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;
import java.util.Date;
import java.util.List;

import javax.ws.rs.Consumes;
import javax.ws.rs.DELETE;
import javax.ws.rs.FormParam;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.PUT;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.QueryParam;
import javax.ws.rs.core.MediaType;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.config.BeanDefinition;
import org.springframework.context.annotation.Scope;
import org.springframework.stereotype.Component;

import io.swagger.annotations.Api;
import io.swagger.annotations.ApiOperation;
import jp.co.auctor.helloworld.common.UuidClass;
import jp.co.auctor.helloworld.entity.Users;
import jp.co.auctor.helloworld.service.UsersService;

@Component
@Path("/users")
@Api(tags = { "/users" })
//@Consumes({ MediaType.APPLICATION_JSON + "; charset=UTF8"}) // リクエストのContent-Typeがapplication/jsonの時に当クラスが実行される
@Produces({ MediaType.APPLICATION_JSON + ";charset=UTF-8" }) // レスポンスのContent-Typeがapplication/jsonの時に当クラスが実行される
@Scope(BeanDefinition.SCOPE_PROTOTYPE)
public class UsersResource {

    @Autowired
    UsersService usersService;

    //@Autowired
    //Users2Service users2Service;

    @Autowired
    public UuidClass uuidClass;

    /**
     * 検索するメソッド。
     */
    @GET
    @Produces({ MediaType.APPLICATION_JSON + ";charset=UTF-8" }) //XMLかJSONを生成
    @ApiOperation(value = "IDが一致するデータを取得")
    public List<Users> search(@QueryParam("id") Long id, @QueryParam("name") String name,
            @QueryParam("email") String email) {
        return usersService.findUsers(id, name, email);
    }

    /**
     * IDで検索するメソッド。
     */
    @GET
    @Path("{id}")
    @Produces({ MediaType.APPLICATION_JSON + ";charset=UTF-8" }) //XMLかJSONを生成
    public Users getUser(@PathParam("id") long id) {
        return usersService.findById(id);
    }

    /**
     * ユーザーを登録するメソッド。
     */
    @POST
    @Path("/add")
    @Consumes(MediaType.APPLICATION_FORM_URLENCODED + ";charset=UTF-8") //HTTPフォームを受け取る
    //@Produces({ MediaType.APPLICATION_JSON + ";charset=UTF-8" }) 
    //@Consumes("multipart/form-data")
    public Users newUser(@FormParam("id") long id, @FormParam("name") String name, @FormParam("email") String email) {
        Users users = new Users();
        users.setId(id);
        String _name;
		try {
			_name = URLDecoder.decode(name, "UTF-8");
		} catch (UnsupportedEncodingException e) {
			_name = "";	
		}
        users.setName(_name);
        users.setEmail(email);
        users.setLank(1);
        
        Date date = new Date();
        long timeInMilliSeconds = date.getTime();
        java.sql.Date date1 = new java.sql.Date(timeInMilliSeconds);
        
        users.setStartdate(date1);
        return usersService.save(users);
    }

    /**
     * ユーザーを変更するメソッド。
     */
    @PUT
    @Path("{id}")
    @Consumes(MediaType.APPLICATION_FORM_URLENCODED)
    public void updateUser(@PathParam("id") long id, @FormParam("name") String name, @FormParam("email") String email) {
        Users users = new Users();
        users.setId(id);
        users.setName(name);
        users.setEmail(email);
        usersService.update(users);
    }

    //IDで削除するメソッド
    @DELETE
    @Path("{id}")
    public void deleteUser(@PathParam("id") long id) {
        usersService.delete(id);
    }
}