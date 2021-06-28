package be.kdg.stemtest.model.repositories

import androidx.lifecycle.LiveData
import androidx.lifecycle.LiveDataReactiveStreams
import be.kdg.stemtest.model.Interactors.PartyDatabaseInteractor
import be.kdg.stemtest.model.Interactors.PartyRemoteInteractor
import be.kdg.stemtest.model.db.StemTestDb
import be.kdg.stemtest.model.entity.Party
import be.kdg.stemtest.model.rest.RemoteDataSource
import io.reactivex.Completable
import io.reactivex.android.schedulers.AndroidSchedulers
import io.reactivex.schedulers.Schedulers
import javax.inject.Inject

class PartyRepository @Inject constructor(  private val partyDatabaseInteractor: PartyDatabaseInteractor,
                                            private val partyRemoteInteractor: PartyRemoteInteractor) {



    fun getParties(): LiveData<List<Party>> {
        val databaseCall= partyDatabaseInteractor.getAllParties()
        return LiveDataReactiveStreams.fromPublisher(databaseCall)
    }

    fun pushParty(party: Party) : Completable {
        val chosenParty=party

        chosenParty.chosen=true

        val pushDatabase = partyDatabaseInteractor.updateParty(chosenParty)
        return partyRemoteInteractor.chosenParty(chosenParty)
            .concatWith(pushDatabase)
    }

    fun getChosenParty(): LiveData<Party> {
        val database = partyDatabaseInteractor.getChosenParty()
            .observeOn(AndroidSchedulers.mainThread())
            .subscribeOn(Schedulers.io())

        return LiveDataReactiveStreams.fromPublisher(database.toFlowable())
    }

    fun getParty(partyName: String): LiveData<Party> {
        val database = partyDatabaseInteractor.getParty(partyName)
            .observeOn(AndroidSchedulers.mainThread())
            .subscribeOn(Schedulers.io())

        return LiveDataReactiveStreams.fromPublisher(database.toFlowable())
    }
}